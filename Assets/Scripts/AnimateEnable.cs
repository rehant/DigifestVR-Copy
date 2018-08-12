using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEnable : MonoBehaviour {

    [SerializeField]
    private float m_MinStartDelay = 0.1f; //Min
    [SerializeField]
    private float m_MaxStartDelay = 0.5f; //Max

    private float startDelay = 0f;

    [SerializeField]
    private float m_SizeTime = 0.5f; //How long to size up

    private Vector3 localScale = new Vector3(0f, 0f, 0f);

    float sizeTimer = 0f; //Time sizing

	// Use this for initialization
	private void Awake()
    {
        localScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        startDelay = Random.Range(m_MinStartDelay, m_MaxStartDelay);
        transform.localScale = Vector3.zero;
        sizeTimer = 0f;
    }

    // Update is called once per frame
    void Update () {

        if (startDelay <= 0f)
        {
            if (sizeTimer < m_SizeTime)
            {
                sizeTimer += 1f * Time.deltaTime;

                if (sizeTimer > m_SizeTime) sizeTimer = m_SizeTime;

                transform.localScale = Vector3.Lerp(Vector3.zero, localScale, sizeTimer / m_SizeTime); //Scale up!
            }
        }
        else
        {
            startDelay -= 1f * Time.deltaTime;
        }
	}
}
