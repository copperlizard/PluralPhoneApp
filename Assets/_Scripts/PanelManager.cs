using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    public Animator m_calcPanel, m_matPanel;

    public float m_minMove = 300.0f;

    // Use this for initialization
    void Start()
    {
        if (m_calcPanel == null)
        {
            Debug.Log("m_calcPanel not assigned!!!");
        }

        if (m_matPanel == null)
        {
            Debug.Log("m_matPanel not assigned!!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        GetKeyboardInput();
#else
        GetTouchInput();
#endif
    }

    private void GetKeyboardInput ()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SwipeLeft();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SwipeRight();
        }
    }

    private void GetTouchInput ()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                //Watch for valid move
                StartCoroutine(GetMoveInput());
                return;
            }
        }
    }

    IEnumerator GetMoveInput ()
    {
        Vector2 touchStart = Input.GetTouch(0).position;

        Vector2 move;

        do
        {
            move = Input.GetTouch(0).position - touchStart;

            if (move.magnitude > m_minMove)
            {                
                if (move.normalized.x >= 0.8f)
                {
                    // Swipe right
                    SwipeRight();
                }
                else if (move.normalized.x <= -0.8f)
                {
                    // Swipe left
                    SwipeLeft();
                }

                // Exit loop
                break;
            }

            yield return null;
        } while (Input.touchCount > 0);

        yield return null;
    }

    private void SwipeRight ()
    {
        m_matPanel.SetBool("Visible", false);
        m_calcPanel.SetBool("Visible", true);
    }

    private void SwipeLeft()
    {
        m_matPanel.SetBool("Visible", true);
        m_calcPanel.SetBool("Visible", false);
    }
}

