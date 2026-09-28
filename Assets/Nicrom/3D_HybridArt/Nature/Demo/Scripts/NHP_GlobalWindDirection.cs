
using UnityEngine;

namespace Nicrom.NHP {
    [ExecuteInEditMode]
    public class NHP_GlobalWindDirection : MonoBehaviour {

        [Range(0, 360)]
        public int windDir = 0;

        // Update is called once per frame
        void Update()
        {
            Shader.SetGlobalFloat("MBGlobalWindDir", windDir);
        }

        void OnDrawGizmos()
        {
            Vector3 dir = Quaternion.Euler(0, windDir, 0) * new Vector3(0,0,1) * 3;
            DrawWindDir(transform.position, dir, 1);
        }

        public void DrawWindDir(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
    }
}
