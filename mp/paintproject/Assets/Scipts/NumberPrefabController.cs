using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPrefabController : MonoBehaviour
{
    public int row, column;
    public int index;

    IEnumerator showRoutine;

    public Vector3 rotateVec;

    public float speed;

    void OnEnable()
    {
        ColorManager.Instance.OnChooseColor += OnChooseColor;
    }

    void OnDisable()
    {
        ColorManager.Instance.OnChooseColor -= OnChooseColor;
    }

    public void OnChooseColor(int _index)
    {
        if(index == _index)
        {
            if(showRoutine != null)
            {
                StopCoroutine(showRoutine);
            }

            showRoutine = ShowRoutine();

            StartCoroutine(showRoutine);
        }
        else
        {
            if(showRoutine != null)
            {
                StopCoroutine(showRoutine);
            }
            showRoutine = null;

            transform.rotation = Quaternion.identity;
        }
    }

    IEnumerator ShowRoutine()
    {
        while(true)
        {
            transform.Rotate(rotateVec * speed * Time.deltaTime);
            yield return null;
        }
    }
}
