using System;
using Gameplay;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Tests.Gameplay
{
    public class Projection : MonoBehaviour
    {
        private Scene m_simulationScene;
        private PhysicsScene2D m_physicsScene;

        [SerializeField] 
        private Tilemap m_ObstaclesParent;

        [SerializeField] private Grid m_Grid;

        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private int m_maxPhysicsFrameIterations;

        private void Start()
        {
            CreatePhysicsScene();
        }

        void CreatePhysicsScene()
        {
            m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            m_physicsScene = m_simulationScene.GetPhysicsScene2D();
            var GhostGrid = Instantiate(m_Grid.gameObject, m_Grid.transform.position, m_Grid.transform.rotation);
            var GhostTileMap = GhostGrid.GetComponentInChildren<TilemapRenderer>();
            GhostTileMap.enabled = false;
            SceneManager.MoveGameObjectToScene(GhostGrid, m_simulationScene);

        }

        public void SimulateTrajectory(BoomsWorth boomsWorth, Vector2 pos, Vector2 veloctiy)
        {
            var GhostBoomsworth = Instantiate(boomsWorth, pos, quaternion.identity);
            GhostBoomsworth.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            Destroy(GhostBoomsworth.transform.GetChild(0).gameObject);
            SceneManager.MoveGameObjectToScene(GhostBoomsworth.gameObject, m_simulationScene);
            //GhostBoomsworth.SendFlying(veloctiy);

            m_lineRenderer.positionCount = m_maxPhysicsFrameIterations;

            for (int i = 0; i < m_maxPhysicsFrameIterations; i++)
            {
                m_physicsScene.Simulate(Time.fixedDeltaTime);
                m_lineRenderer.SetPosition(i, GhostBoomsworth.transform.position);
            }
            
            Destroy(GhostBoomsworth);
        }
    }
}