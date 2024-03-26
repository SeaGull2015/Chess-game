using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class promotionSelectScript : MonoBehaviour
{
    public TMP_Dropdown myDown;
    private PieceBehaviour caller;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNewSelect(float x, float y, PieceBehaviour whoCalled)
    {
        transform.position = new Vector3(x, y, 0);
        caller = whoCalled;
        gameObject.SetActive(true);
        myDown.onValueChanged.AddListener(delegate
        {
            onSelection(myDown);
        });
    }

    private void onSelection(TMP_Dropdown change)
    {
        caller.recallPromotion(change.options[change.value].text.ToLower());
        gameObject.SetActive(false);
    }
}
