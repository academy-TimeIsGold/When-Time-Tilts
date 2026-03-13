using System;
using UnityEditor;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class SavePoint : MonoBehaviour
{
    [Header("세이브 포인트 데이터")]
    [Tooltip("이 세이브 포인트의 고정값 SO 연결. playerPosition은 SO에 직접 입력")]
    public SavePointData savePointData;

    [Tooltip("true: 중간 체크 포인트(위치만 기억, 파일 저장 X)\nfalse: 스테이지 세이브 포인트 (파일 저장O)")]
    [SerializeField] private bool isCheckpoint = false;

    [Header("세이브 포인트 활성화")]
    [Tooltip("세이브 포인트가 켜졌을 때 보여줄 스프라이트 오브젝트")]
    public GameObject activeSprite;

    [Header("카메라 세팅")]
    [Tooltip("해당 세이브 포인트가 있는 방의 테두리(Collider2D)")]
    public Collider2D roomBounds;

    [Tooltip("true = 씬 시작 시 자동으로 세이브 포인트 등록 (첫 번째 세이브 포인트에 체크)")]
    [SerializeField] private bool isStartPoint = false;

    private bool isActivated = false;   //SavePoint 비활성화 상태

    private void Start()
    {
        // 첫 게임 시작 시 자동 등록
        if (isStartPoint && !isCheckpoint && GameManager.Instance?.currentSavePointData == null)
        {
            GameManager.Instance.UpdateSavePoint(this, savePointData);
            Debug.Log($"[SavePoint] {gameObject.name} 시작 세이브 포인트 자동 등록");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (isActivated) return;

        ActiveteSavePoint();
    }

    public void ActiveteSavePoint()
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

        if (isCheckpoint)
        {
            // 체크 포인트 위치 전달
            GameManager.Instance?.RegisterCheckpoint(this);
        }
    }

    //다른 SavePoint가 활성화 되면 기존 SavePoint의 불은 꺼짐
    public void DeactivateSavePoint()
    {
        isActivated = false;
        if (activeSprite != null) activeSprite.SetActive(false);
    }               
}
