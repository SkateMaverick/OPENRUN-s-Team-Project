using UnityEngine;

public class ScaleChange : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // 쉐이프 키를 가지고 있는 메쉬
    public Vector3 targetLengthSize; // Stretch_Length 쉐이프 키를 최대로 적용했을 때의 콜라이더 사이즈
    public Vector3 targetWidthSize; // Stretch_Width 쉐이프 키를 최대로 적용했을 때의 콜라이더 사이즈

    private BoxCollider _boxCollider;
    private Vector3 _initialBoxColliderSize; // 초기 박스 콜라이더 사이즈
    private Vector3 _changedBoxColliderSize; // 런타임에서 변경되는 박스 콜라이더의 사이즈를 저장하는 용도
    private int _lengthIndex; // Stretch_Length 쉐이프 키의 인덱스 번호
    private int _widthIndex; // Stretch_Width 쉐이프 키의 인덱스 번호
    private float _currentLengthWeight; // 런타임에서 Stretch_Length 쉐이프 키의 가중치
    private float _currentWidthWeight; // 런타임에서 Stretch_Width 쉐이프 키의 가중치

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _initialBoxColliderSize = _boxCollider.size; // 초기 기본 콜라이더 사이즈 저장
        
        _lengthIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Stretch_Length");
        _widthIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Stretch_Width");
    }

    // 크기조절 UI에 직접 연결해야 함
    public void OnSliderValueChanged(float value)
    {
        float changeValue; // 쉐이프 키 가중치로 들어갈 값
        
        if (value < 0.5f)
        {
            changeValue = (0.5f - value) * 200f; // 0~0.5를 0~100으로
            
            skinnedMeshRenderer.SetBlendShapeWeight(_lengthIndex, changeValue); // changeValue를 쉐이프 키 가중치로 설정
            skinnedMeshRenderer.SetBlendShapeWeight(_widthIndex, 0);
            
            // 콜라이더 크기 변경
            float lengthDeltaSizeX = _initialBoxColliderSize.x - targetLengthSize.x;
            float lengthDeltaSizeY =  _initialBoxColliderSize.y - targetLengthSize.y;
            float lengthDeltaSizeZ =  _initialBoxColliderSize.z - targetLengthSize.z;

            _changedBoxColliderSize.x = targetLengthSize.x + (value * 2 * lengthDeltaSizeX);
            _changedBoxColliderSize.y = targetLengthSize.y + (value * 2 * lengthDeltaSizeY);
            _changedBoxColliderSize.z = targetLengthSize.z + (value * 2 * lengthDeltaSizeZ);
            
            _boxCollider.size = _changedBoxColliderSize; // 콜라이더 사이즈를 변경된 사이즈로 변경
        }
        else
        {
            changeValue = (value - 0.5f) * 200f; // 0.5~1을 0~100으로
            
            skinnedMeshRenderer.SetBlendShapeWeight(_lengthIndex, 0);
            skinnedMeshRenderer.SetBlendShapeWeight(_widthIndex, changeValue);
            
            // 콜라이더 크기 변경
            float widthDeltaSizeX = targetWidthSize.x - _initialBoxColliderSize.x;
            float widthDeltaSizeY = targetWidthSize.y - _initialBoxColliderSize.y;
            float widthDeltaSizeZ = targetWidthSize.z - _initialBoxColliderSize.z;

            _changedBoxColliderSize.x = _initialBoxColliderSize.x + ((value - 0.5f) * 2 * widthDeltaSizeX);
            _changedBoxColliderSize.y = _initialBoxColliderSize.y + ((value - 0.5f) * 2 * widthDeltaSizeY);
            _changedBoxColliderSize.z = _initialBoxColliderSize.z + ((value - 0.5f) * 2 * widthDeltaSizeZ);
            
            _boxCollider.size = _changedBoxColliderSize;
        }

        //_currentLengthWeight = skinnedMeshRenderer.GetBlendShapeWeight(_lengthIndex);
        //_currentWidthWeight = skinnedMeshRenderer.GetBlendShapeWeight(_widthIndex);
    }
}