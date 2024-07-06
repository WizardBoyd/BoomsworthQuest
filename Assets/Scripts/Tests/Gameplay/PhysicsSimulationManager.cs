using System;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using Misc.ExtensionMethods;
using Misc.Singelton;
using SceneManagment;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace Tests.Gameplay
{

    public class PhysicsPool
    {
        private List<GameObject> m_pooledGameObjects;
        private List<GameObject> m_lentObject;
        private bool bIsPrewarmed = false;
        private Scene m_physicsScene;
        private Transform m_RootPoolTransform;

        public PhysicsPool(Scene physicsScene)
        {
            m_pooledGameObjects = ListPool<GameObject>.Get();
            m_lentObject = ListPool<GameObject>.Get();
            m_physicsScene = physicsScene;
            Prewarm(10);
        }
        ~PhysicsPool()
        {
            //Should not need to clear as when the physics scene unloads all the game
            //Objects should go with it
            ListPool<GameObject>.Release(m_pooledGameObjects);
            ListPool<GameObject>.Release(m_lentObject);
        }
        
        private void Prewarm(int num)
        {
            if(bIsPrewarmed)
                return;

            GameObject RootPhysicsPoolObject = new GameObject("PhysicsPool");
            m_RootPoolTransform = RootPhysicsPoolObject.transform;
            m_RootPoolTransform.position = Vector3.zero;
            m_RootPoolTransform.rotation = Quaternion.identity;
            SceneManager.MoveGameObjectToScene(RootPhysicsPoolObject, m_physicsScene);

            for (int i = 0; i < num; i++)
            {
                GameObject newObj = Create();
                newObj.SetActive(false);
                m_pooledGameObjects.Add(newObj);
            }
            bIsPrewarmed = true;
        }

        public GameObject Request(PhysicSimulationRequestDetails details)
        {
            bool NewlyCreate = false;
            GameObject requestObject = null;
            if(!bIsPrewarmed)
                Prewarm(5);
            if (!DoesAnyPooledObjectHaveRequirements(details))
            {
                requestObject = Create(details);
                NewlyCreate = true;
            }
            else
            {
                requestObject = GetPooledObjectWithRequirements(details);
            }

            if (!NewlyCreate)
            {
                m_pooledGameObjects.Remove(requestObject);
            }
            m_lentObject.Add(requestObject);
            
            requestObject.SetActive(true);
            requestObject.transform.SetParent(null);
            
            return requestObject;
        }

        public void Return(IEnumerable<GameObject> members)
        {
            foreach (GameObject member in members)
            {
                Return(member);
            }
        }
        
        public void Return(GameObject member)
        {
            if (!m_lentObject.Contains(member))
            {
                Debug.LogWarning("Can't Return object to pool because it does not belong Try Destroying it instead");
                return;
            }
            if (!bIsPrewarmed)
                bIsPrewarmed = true;
            if (member.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            member.transform.SetParent(m_RootPoolTransform);
            member.SetActive(false);
            m_lentObject.Remove(member);
            m_pooledGameObjects.Add(member);
        }

        private GameObject Create(PhysicSimulationRequestDetails details)
        {
            GameObject newObj = Create();
            newObj.AddComponent(details.Collider2DType);
            if (details.NeedRigidBody)
            {
                Rigidbody2D rb =  newObj.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            return newObj;
        }

        private GameObject Create()
        {
            GameObject newObj = new GameObject(string.Concat("PhysicsObj_", 
                m_pooledGameObjects.Count.ToString()));
            SceneManager.MoveGameObjectToScene(newObj, m_physicsScene);
            newObj.transform.SetParent(m_RootPoolTransform);
            
            return newObj;
        }

        private GameObject GetPooledObjectWithRequirements(PhysicSimulationRequestDetails details)
        {
            if (details.NeedRigidBody)
            {
                return m_pooledGameObjects.First(x =>
                {
                    if (x.TryGetComponent<Rigidbody2D>(out _) && x.TryGetComponent(details.Collider2DType, out _))
                        return true;
                    return false;
                });
            }
            else
            {
                return m_pooledGameObjects.First(x => x.TryGetComponent(details.Collider2DType, out _));
            }
        }
        private bool DoesAnyPooledObjectHaveRequirements(PhysicSimulationRequestDetails details)
        {
            if (details.NeedRigidBody)
            {
                return m_pooledGameObjects.Any(x =>
                {
                    if (x.TryGetComponent<Rigidbody2D>(out _) && x.TryGetComponent(details.Collider2DType, out _))
                        return true;
                    return false;
                });
            }
            else
            {
                return m_pooledGameObjects.Any(x => x.TryGetComponent(details.Collider2DType, out _));
            }
        }
    }
    
    public struct PhysicSimulationRequestDetails
    {
        public Type Collider2DType;
        public bool NeedRigidBody;
    }
    
    public class PhysicsSimulationScene2D
    {
        public Scene SimulationScene { get; private set; }
        public PhysicsScene2D PhysicsScene2D { get; private set; }
        
        private Dictionary<GameObject, GameObject> m_registeredPhysicsObjects;
        private HashSet<int> m_CurrentFrameInstanceIds;
        
        public PhysicsSimulationScene2D(string sceneName)
        {
            m_registeredPhysicsObjects = new Dictionary<GameObject, GameObject>();
            SimulationScene =
                SceneManager.CreateScene(sceneName, new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            PhysicsScene2D = SimulationScene.GetPhysicsScene2D();
            m_CurrentFrameInstanceIds = new HashSet<int>();
        }

        ~PhysicsSimulationScene2D()
        {
            SceneLoader.Instance.UnloadPhysicsScene(SimulationScene);
        }

        //public GameObject RequestPhysicsBodyForRequest(PhysicSimulationRequestDetails requestDetails) => PhysicsPool.Request(requestDetails);

        private bool GetCorrelatingObject(GameObject outOfSceneGameObject, out GameObject inPhysicsSceneObj)
        {
            inPhysicsSceneObj = null;
            if (m_registeredPhysicsObjects.ContainsKey(outOfSceneGameObject))
            {
                inPhysicsSceneObj = m_registeredPhysicsObjects[outOfSceneGameObject];
                return true;
            }
            return false;
        }

        private bool GetPhysicsBody(GameObject inPhysicsSceneObj, out Rigidbody2D rb)
        {
            rb = inPhysicsSceneObj.GetComponent<Rigidbody2D>();
            if (rb != null)
                return true;
            return false;
        }

        public void AddObjects(IEnumerable<GameObject> outOfSceneGameObjects) => outOfSceneGameObjects.ForEach(AddObject);
        public void AddObject(GameObject outOfSceneGameObject)
        {
            if (!m_registeredPhysicsObjects.ContainsKey(outOfSceneGameObject))
            {
                GameObject ghostObject = null;
                CreatePhysicsSimulationObject(outOfSceneGameObject, out ghostObject);
                if(ghostObject != null)
                    m_registeredPhysicsObjects.Add(outOfSceneGameObject, ghostObject);
            }
        }

        public void DestroyObjects(GameObject[] outOfSceneGameObjects) => outOfSceneGameObjects.ForEach(DestroyObject);
        public void DestroyObject(GameObject outOfSceneGameObject)
        {
            if (m_registeredPhysicsObjects.ContainsKey(outOfSceneGameObject))
            {
                GameObject toDestroy = m_registeredPhysicsObjects[outOfSceneGameObject];
                m_registeredPhysicsObjects.Remove(outOfSceneGameObject);
               Object.Destroy(toDestroy);
            }
        }

        public void SimulatePhysicsObject()
        {
            foreach (var (outOfSceneObj, inPhysicsSceneObj) in m_registeredPhysicsObjects)
            {
                inPhysicsSceneObj.transform.position = outOfSceneObj.transform.position;
                inPhysicsSceneObj.transform.rotation = outOfSceneObj.transform.rotation;
                inPhysicsSceneObj.transform.localScale = outOfSceneObj.transform.localScale;
            }

            PhysicsScene2D.Simulate(Time.fixedDeltaTime);
        }

        public void MoveAllPendingObjectToScene()
        {
            if (m_CurrentFrameInstanceIds.Count > 0)
            {
                NativeArray<int> instanceIds =
                    new NativeArray<int>(m_CurrentFrameInstanceIds.ToArray(), Allocator.Temp);
                SceneManager.MoveGameObjectsToScene(instanceIds, SimulationScene);
                m_CurrentFrameInstanceIds.Clear();
            }
        }

        private void CreatePhysicsSimulationObject(GameObject outOfSceneGameObject, out GameObject inPhysicsGameObject)
        {
            inPhysicsGameObject = Object.Instantiate(outOfSceneGameObject, outOfSceneGameObject.transform.position,
                outOfSceneGameObject.transform.rotation);
            StripVisuals(inPhysicsGameObject);
            m_CurrentFrameInstanceIds.Add(inPhysicsGameObject.GetInstanceID());
        }

        private void StripVisuals(GameObject ghostObj)
        {
            ghostObj.RemoveAllComponentsFromChildrenAndSelf(
                typeof(Collider2D),
                typeof(CircleCollider2D),
                typeof(Rigidbody2D),
                typeof(TilemapCollider2D),
                typeof(Tilemap),
                typeof(Grid));
            //Move the ghostObjectLayer to ignore raycasts
            ghostObj.AssignAllGameObjectAndChildrenToLayer(LayerMask.NameToLayer("Ignore Raycast"));
        }
    }
    
    public class PhysicsSimulationManager : MonoBehaviorSingleton<PhysicsSimulationManager>
    {
        private PhysicsSimulationScene2D m_PhysicsSimulationScene2D;
        private PhysicsPool m_physicsPool;

        [Header("Configurations")] 
        [SerializeField]
        private int m_simulationSteps = 10;
        
        [Header("Listening To")] [SerializeField]
        private VoidEventChannelSO on_SceneReady = default;

        protected override void Awake()
        {
            base.Awake();
            m_PhysicsSimulationScene2D = new PhysicsSimulationScene2D("SimulationScene");
            m_physicsPool = new PhysicsPool(m_PhysicsSimulationScene2D.SimulationScene);
        }

        private void OnEnable()
        {
            on_SceneReady.OnEventRaised += OnGameSceneReady;
        }

        private void OnDisable()
        {
            on_SceneReady.OnEventRaised -= OnGameSceneReady;
        }

        private void OnGameSceneReady()
        {
            List<GameObject> physicObjects = null;
            GetAllPhysicObjectsInGameScene(out physicObjects);
            m_PhysicsSimulationScene2D.AddObjects(physicObjects);
        }

        private void GetAllPhysicObjectsInGameScene(out List<GameObject> physicObjects)
        {
            physicObjects = ListPool<GameObject>.Get();
            IEnumerable<GameObject> taggedPhysicObjects = GameObject.FindGameObjectsWithTag("PhysicObject").Where(x =>
            {
                if (x.transform.parent == null)
                    return true;
                return false;
            });
            foreach (GameObject taggedPhysicObject in taggedPhysicObjects)
            {
                physicObjects.Add(taggedPhysicObject);
            }
        }

        private void Update()
        {
            //Spawn any new Physic Objects
            
            
            //Update The Objects as in set forces, etc...
        }

        private void LateUpdate()
        {
            m_PhysicsSimulationScene2D.MoveAllPendingObjectToScene();
        }

        public void SimulateTrajectory(Vector2 position, Vector2 velocity, LineRenderer visualRenderer = null)
        {
            GameObject trajectoryObj = new GameObject("TrajcetoryObject");
            trajectoryObj.transform.position = new Vector3(position.x, position.y, trajectoryObj.transform.position.z);
            Collider2D collider2D = trajectoryObj.AddComponent<CircleCollider2D>();
            Rigidbody2D rb = trajectoryObj.AddComponent<Rigidbody2D>();
            SceneManager.MoveGameObjectToScene(trajectoryObj, m_PhysicsSimulationScene2D.SimulationScene);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(velocity);
            if (visualRenderer != null)
            {
                visualRenderer.positionCount = m_simulationSteps;
            }

            for (int i = 0; i < m_simulationSteps; i++)
            {
                m_PhysicsSimulationScene2D.PhysicsScene2D.Simulate(Time.fixedDeltaTime);
                if(visualRenderer != null)
                    visualRenderer.SetPosition(i, trajectoryObj.transform.position);
                Debug.Log($"Trajectory Object Position {trajectoryObj.transform.position}");
            }
            Destroy(trajectoryObj);
            //m_physicsPool.Return(trajectoryObj);
        }
    }
}