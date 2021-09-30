using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Collectable : MonoBehaviour
{
    
    public enum Type
    {
        Apple,
        Carrot
    }

    public Type type;


    private Transform obj;
    private Transform points;
    private TextMeshPro text;

    private Dictionary<Type, int> collectables;

    private void Awake()
    {
        DOTween.Init();

        obj = transform.Find("Object");
        points = transform.Find("Points");
        text = points.GetComponent<TextMeshPro>();

        points.gameObject.SetActive(false);
    }

    void Start()
    {
        collectables = new Dictionary<Type, int>()
        {
            { Type.Apple, 1000 },
            { Type.Carrot, 500 }
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (obj.gameObject.activeSelf)
            {
                obj.gameObject.SetActive(false);
            }

            if (!points.gameObject.activeSelf)
            {
                points.gameObject.SetActive(true);

                //TODO: player score update

                //TODO: create DOTween sequence which contains all tweens
                points.transform.DOMoveY(transform.position.y + 2, 1);

                float alpha = text.color.a;
                float fadeTime = 2.0f;
                DOTween.To(() => alpha, x => alpha = x, 0.0f, fadeTime).OnUpdate(() => UpdateText(alpha)).OnComplete(Destroy);
            }
        }
        print("Collision between " + this.name + " and " + collision.gameObject.name);
    }

    private void Destroy()
    {
        print("Destroying gameobject: " + gameObject.name);
        Destroy(gameObject);
    }

    private void UpdateText(float value)
    {
        Color c = text.color;
        c.a = value;
        text.color = c;
    }

    void Update()
    {
        
    }
}
