using UnityEngine;
using System.Collections;

namespace TSA
{
    static public class ResourceManager
    {
        //event delegates
        public delegate void PlayerEventHandler();

        static public event PlayerEventHandler playerFound;
        static public event PlayerEventHandler playerClearedLevel;

        //constant values
        public static readonly Vector3 OUTOFBOUNDS = new Vector3(0, -999, 0);

        //scene values
        public static Vector3 SceneLLBounds;
        public static Vector3 SceneURBounds;
        

        public static void CallplayerFound()
        {
            if (playerFound != null)
                playerFound();
        }

        public static void CallplayerClearedLevel()
        {
            if (playerClearedLevel != null)
                playerClearedLevel();
        }

        public static Vector3 MakeCameraVecInBounds(Vector3 target, float camzoom)
        {
            
            Vector3 CamURBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camzoom));
            Vector3 CamLLBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camzoom));

            float CamXOffSet = (Mathf.Abs(CamURBound.x - CamLLBounds.x) / 2);
            float CamZOffSet = (Mathf.Abs(CamURBound.z - CamLLBounds.z) / 2);

            float ZMin = ResourceManager.SceneLLBounds.z + CamZOffSet;
            float ZMax = ResourceManager.SceneURBounds.z - CamZOffSet;
            float XMax = ResourceManager.SceneURBounds.x - CamXOffSet;
            float XMin = ResourceManager.SceneLLBounds.x + CamXOffSet;
            /*            
            float ZMin = ResourceManager.SceneLLBounds.z + 3;
            float ZMax = ResourceManager.SceneURBounds.z - 3;
            float XMax = ResourceManager.SceneURBounds.x - 3;
            float XMin = ResourceManager.SceneLLBounds.x + 3;
            */

            //Debug.Log(ZMin);
            //Debug.Log(ZMax);
            

            target.x = (target.x < XMin) ? XMin : (target.x > XMax) ? XMax : target.x;
            target.z = (target.z < ZMin) ? ZMin : (target.z > ZMax) ? ZMax : target.z;
            
            

            return target;
        }

    }

    
}
