using System;
using UnityEditor;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class SavePoint : MonoBehaviour
{
    [Header("세이브 포인트 데이터")]
    [Tooltip("이 세이브 포인트의 고정값 SO 연결. playerPosition은 SO에 직접 입력")]
    public SavePointData savePointData;

    [Header("세이브 포인트 활성화")]
    [Tooltip("세이브 포인트가 켜졌을 때 보여줄 스프라이트 오브젝트")]
    public GameObject activeSprite;

    [Header("카메라 세팅")]
    [Tooltip("해당 세이브 포인트가 있는 방의 테두리(Collider2D)")]
    public Collider2D roomBounds;

    private bool isActivated = false;   //SavePoint 비활성화 상태

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isActivated)
        {
            ActiveteSavePoint();
        }
    }

    private void ActiveteSavePoint()
    {
        //SavePoint 활성화
        isActivated = true;
        Debug.Log("SavePoint 활성화 완료");

        if (activeSprite != null)
        {
            //SavePoint 이미지 활성화
            activeSprite.SetActive(true);

            //(혹시 애니메이션 쓸까봐 추가)
            //activeSprite.GetComponent<Animator>().SetTrigger("Activate");
        }

        if (savePointData == null)
        {
            Debug.LogWarning($"[SavePoint] {gameObject.name}: SavePointData가 연결되지 않았습니다.");
            return;
        }

        //SavePoint의 부활 좌표를 넘김
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateSavePoint(this, savePointData);
        }

        //카메라가 따라옴
        if (CameraManager.Instance != null || roomBounds != null)
        {
            CameraManager.Instance.SnapToNewStage(roomBounds);
        }
    }

    //다른 SavePoint가 활성화 되면 기존 SavePoint의 불은 꺼짐
    public void DeactivateSavePoint()
    {
        isActivated = false;
        if (activeSprite != null) activeSprite.SetActive(false);
    }               
}
