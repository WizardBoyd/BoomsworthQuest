using System;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using Misc.ExtensionMethods;
using Misc.Singelton;
using Pool;
using SceneManagment;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace Tests.Gameplay
{

    public enum Collider2DType
    {
        Box,
        Circle,
        Capsule
    }
    
    public abstract class TrajectorySimulationRequest
    {
        public Vector2 OriginPoint;
        public Vector2 Force;
        public int SimulationSteps;
        public LineRenderer Renderer;
    }

    public class TrajectorySimulationRequestNoObject : TrajectorySimulationRequest
    {
        public Collider2DType Type;
    }

    public class TrajectorySimulationRequestObject : TrajectorySimulationRequest
    {
        public GameObject cloneObject;
    }

    public class PhysicsObject
    {
        public PhysicsObject(Transform transform, Rigidbody2D rb, Collider2D collider)
        {
            this.PhysicObjectTransform = transform;
            this.Rigidbody2D = rb;
            this.Collider2D = collider;
        }
        
        public Transform PhysicObjectTransform;
        public Rigidbody2D Rigidbody2D;
        public Collider2D Collider2D;
    }
    
    public class PhysicsPool : IPool<PhysicsObject>
    {
        private Stack<PhysicsObject> m_pooledObjects;
        private bool bIsPrewarmed = false;
        private Scene m_physicsScene;
        private Transform m_RootPoolTransform;

        public PhysicsPool(Scene physicsScene)
        {
            m_pooledObjects = new Stack<PhysicsObject>();
            m_physicsScene = physicsScene;
        }
        
        public void Prewarm(int num)
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
                PhysicsObject newObj = Create(Collider2DType.Circle);
                m_pooledObjects.Push(newObj);
            }
            bIsPrewarmed = true;
        }

        public PhysicsObject Request()
        {
            PhysicsObject requestedObject = null;
            if (m_pooledObjects.Count > 0)
            {
                requestedObject = m_pooledObjects.Pop();
                requestedObject.PhysicObjectTransform.gameObject.SetActive(true);
                requestedObject.PhysicObjectTransform.SetParent(null);
            }
            else
            {
                requestedObject = Create(Collider2DType.Circle);
                requestedObject.PhysicObjectTransform.SetParent(null);
            }
            return requestedObject;
        }

        public PhysicsObject Request(Collider2DType type)
        {
            PhysicsObject requestedObject = null;
            if (m_pooledObjects.Count > 0)
            {
                requestedObject = m_pooledObjects.FirstOrDefault(x =>
                {
                    Type colliderType = x.Collider2D.GetType();
                    switch (type)
                    {
                        case Collider2DType.Box:
                            if (colliderType == typeof(BoxCollider2D))
                                return true;
                            break;
                        case Collider2DType.Circle:
                            if (colliderType == typeof(CircleCollider2D))
                                return true;
                            break;
                        case Collider2DType.Capsule:
                            if (colliderType == typeof(CapsuleCollider2D))
                                return true;
                            break;
                    }
                    return false;
                });
                if (requestedObject == null)
                {
                    requestedObject = Create(type);
                }
                requestedObject.PhysicObjectTransform.gameObject.SetActive(true);
                requestedObject.PhysicObjectTransform.SetParent(null);
            }
            else
            {
                requestedObject = Create(type);
                requestedObject.PhysicObjectTransform.SetParent(null);
            }
            return requestedObject;
        }

        public void Return(PhysicsObject member)
        {
            member.PhysicObjectTransform.gameObject.SetActive(false);
            member.PhysicObjectTransform.SetParent(m_RootPoolTransform);
            member.PhysicObjectTransform.position = Vector3.zero;
        }

        
        
        private PhysicsObject Create(Collider2DType type)
        {
            GameObject newObj = new GameObject(string.Concat("PhysicsObj_", 
                m_pooledObjects.Count.ToString()));
            Rigidbody2D rb = newObj.AddComponent<Rigidbody2D>();
            Collider2D collider2D = null;
            switch (type)
            {
                case Collider2DType.Box:
                    collider2D = newObj.AddComponent<BoxCollider2D>();
                    break;
                case Collider2DType.Circle:
                    collider2D = newObj.AddComponent<CircleCollider2D>();
                    break;
                case Collider2DType.Capsule:
                    collider2D = newObj.AddComponent<CapsuleCollider2D>();
                    break;
            }
            SceneManager.MoveGameObjectToScene(newObj, m_physicsScene);
            newObj.transform.SetParent(m_RootPoolTransform);
            newObj.SetActive(false);
            return new PhysicsObject(newObj.transform, rb, collider2D);
        }

    }
    
    public class PhysicsSimulationManager : MonoBehaviorSingleton<PhysicsSimulationManager>
    {
        private Scene m_simulationScene;
        private PhysicsScene2D m_PhysicsScene;

        private PhysicsPool m_pool;
        
        [Header("Listening To")] [SerializeField]
        private VoidEventChannelSO on_SceneReady = default;

        protected override void Awake()
        {
            base.Awake();
            CreateSceneParameters createSceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
            m_simulationScene = SceneManager.CreateScene("SimulationScene", createSceneParameters);
            m_PhysicsScene = m_simulationScene.GetPhysicsScene2D();
            m_pool = new PhysicsPool(m_simulationScene);
        }

        private void OnEnable()
        {
            on_SceneReady.OnEventRaised += OnGameSceneReady;
            m_pool.Prewarm(10);
        }

        private void OnDisable()
        {
            on_SceneReady.OnEventRaised -= OnGameSceneReady;
        }

        private void OnGameSceneReady()
        {
            List<GameObject> physicObjects = null;
            GetAllPhysicObjectsInGameScene(out physicObjects);
            ReplicateObstacles(ref physicObjects);
            ListPool<GameObject>.Release(physicObjects);
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
        private void ReplicateObstacles(ref List<GameObject> objects)
        {
            int[] ids = new int[objects.Count];
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject ghostObj = Instantiate(objects[i], objects[i].transform.position, objects[i].transform.rotation);
                ghostObj.RemoveAllComponentsFromChildrenAndSelf(
                    typeof(Grid),
                    typeof(Tilemap),
                    typeof(TilemapCollider2D)
                );
                ids[i] = ghostObj.GetInstanceID();
            }
            NativeArray<int> toMoveObj = new NativeArray<int>(ids, Allocator.Temp);
            SceneManager.MoveGameObjectsToScene(toMoveObj, m_simulationScene);
        }

        private void Update()
        {
            //Spawn any new Physic Objects
            
            
            //Update The Objects as in set forces, etc...
        }

        private void LateUpdate()
        {
         
        }

        #region Simulate Trajectory
        
        public void SimulateTrajectory(TrajectorySimulationRequestObject request)
        {
            GameObject trajectoryObj = Instantiate(request.cloneObject);
            trajectoryObj.transform.position = new Vector3(request.OriginPoint.x, request.OriginPoint.y, trajectoryObj.transform.position.z);
            Rigidbody2D rb = trajectoryObj.GetComponent<Rigidbody2D>();
            SceneManager.MoveGameObjectToScene(trajectoryObj, m_simulationScene);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(request.Force, ForceMode2D.Impulse);
            if (request.Renderer != null)
            {
                request.Renderer.positionCount = request.SimulationSteps + 1;
                request.Renderer.SetPosition(0, new Vector3(request.OriginPoint.x, request.OriginPoint.y));
            }

            for (int i = 1; i < request.SimulationSteps + 1; i++)
            {
                m_PhysicsScene.Simulate(Time.fixedDeltaTime);
                if(request.Renderer != null)
                    request.Renderer.SetPosition(i, trajectoryObj.transform.position);
                Debug.Log($"Trajectory Object Position {trajectoryObj.transform.position}");
            }
            Destroy(trajectoryObj);
        }
        // public void SimulateTrajectory(TrajectorySimulationRequestNoObject request)
        // {
        //     Rigidbody2D rb = null;
        //     GameObject trajectoryObj = CreateGameObjectForRequest(request, out rb);
        //     trajectoryObj.transform.position = new Vector3(request.OriginPoint.x, request.OriginPoint.y, trajectoryObj.transform.position.z);
        //     SceneManager.MoveGameObjectToScene(trajectoryObj, m_simulationScene);
        //     
        //     rb.AddForce(request.Force, ForceMode2D.Impulse);
        //     if (request.Renderer != null)
        //     {
        //         request.Renderer.positionCount = request.SimulationSteps + 1;
        //         request.Renderer.SetPosition(0, new Vector3(request.OriginPoint.x, request.OriginPoint.y));
        //     }
        //     
        //     for (int i = 1; i < request.SimulationSteps + 1; i++)
        //     {
        //         m_PhysicsScene.Simulate(Time.fixedDeltaTime);
        //         if(request.Renderer != null)
        //             request.Renderer.SetPosition(i, trajectoryObj.transform.position);
        //     }
        //     Destroy(trajectoryObj);
        //     //m_physicsPool.Return(trajectoryObj);
        // }
        
        public void SimulateTrajectory(TrajectorySimulationRequestNoObject request)
        {
            PhysicsObject physicsObject = m_pool.Request(request.Type);
            Rigidbody2D rb = physicsObject.Rigidbody2D;
            physicsObject.PhysicObjectTransform.position = new Vector3(request.OriginPoint.x, request.OriginPoint.y, physicsObject.PhysicObjectTransform.position.z);
            
            rb.AddForce(request.Force, ForceMode2D.Impulse);
            if (request.Renderer != null)
            {
                request.Renderer.positionCount = request.SimulationSteps + 1;
                request.Renderer.SetPosition(0, new Vector3(request.OriginPoint.x, request.OriginPoint.y));
            }
            
            for (int i = 1; i < request.SimulationSteps + 1; i++)
            {
                m_PhysicsScene.Simulate(Time.fixedDeltaTime);
                if(request.Renderer != null)
                    request.Renderer.SetPosition(i, physicsObject.PhysicObjectTransform.position);
            }
            m_pool.Return(physicsObject);
            //m_physicsPool.Return(trajectoryObj);
        }

        #region Simulate Trajectory Helper Methods

        // private GameObject CreateGameObjectForRequest(TrajectorySimulationRequestNoObject request, out Rigidbody2D rb)
        // {
        //     GameObject trajectoryObj = new GameObject("TrajectoryObject");
        //     rb = trajectoryObj.AddComponent<Rigidbody2D>();
        //     switch (request.Type)
        //     {
        //         case Collider2DType.Box:
        //             trajectoryObj.AddComponent<BoxCollider2D>();
        //             break;
        //         case Collider2DType.Circle:
        //             trajectoryObj.AddComponent<CircleCollider2D>();
        //             break;
        //         case Collider2DType.Capsule:
        //             trajectoryObj.AddComponent<CapsuleCollider2D>();
        //             break;
        //     }
        //     return trajectoryObj;
        // }

        #endregion
        
        #endregion
    }
}