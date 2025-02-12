using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//������ ����â
public class Desc_Nd : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public GameObject m_DescObj; //����â ������Ʈ
    public Text m_NameTxt; //�̸�
    public Text m_DescText; //����â �ؽ�Ʈ
    public Image m_Icon; //������
    public Button m_CloseBtn;
    public GameObject m_NameBar;

    Vector2 offset;

    #region Singleton
    public static Desc_Nd Inst;
    void Awake()
    {
        if (Inst == null)
            Inst = this;
    }
    #endregion

    void Start()
    {
        m_DescObj.SetActive(false);
        m_CloseBtn.onClick.AddListener(() => m_DescObj.SetActive(false));

        m_DescText.text = "";
        m_NameTxt.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            m_DescObj.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out offset);

        // Calculate the offset between the mouse position and the object's position
        offset = (Vector2)m_DescObj.GetComponent<RectTransform>().position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            m_DescObj.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // Update the object's position to follow the mouse pointer
            m_DescObj.GetComponent<RectTransform>().position = eventData.position + offset;
        }
    }

}
