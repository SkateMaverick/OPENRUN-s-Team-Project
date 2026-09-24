using UnityEngine;

[CreateAssetMenu(fileName = "WalkPoseSet", menuName = "ScriptableObject/Walk Pose Set")]
public class WalkPoseSet : ScriptableObject
{
    [System.Serializable]
    public class WalkKeyframe
    {
        public Vector3 leadTargetPosition;
        public Vector3 leadHintPosition;
        public Vector3 followTargetPosition;
        public Vector3 followHintPosition;
        public float duration;
    }

    public WalkKeyframe[] keyframes;
}
