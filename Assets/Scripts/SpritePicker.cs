using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePicker : MonoBehaviour
{

    public Sprite[] sprites = new Sprite[32];
    public int currentSpriteIndex;

    private void Start()
    {
        if (this.gameObject.name != "ToggleBlock" && this.gameObject.name != "ToggleBlockEmpty")
            Destroy(this);
    }

    public void PickSprite(string tilename)
    {
        if (tilename != this.gameObject.name)
            return;
        int spritePicker = 0;
        int num = 1;
        Vector2 pos = this.gameObject.transform.position;

        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x * y == 0 && x + y != 0)
                {
                    Vector2 newpos = new Vector2(pos.x + x, pos.y + y);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(newpos, Vector2.zero);
                    foreach(RaycastHit2D hit in hits)
                    {
                        if (this.gameObject.GetComponent<Ground>() != null || tilename == "Bridge")
                        {
                            if (hit.collider != null && hit.collider.gameObject.GetComponent<Ground>() != null)
                            {
                                spritePicker += num;
                            }
                        }
                        else
                        {
                            if (hit.collider != null && hit.collider.gameObject.name == tilename)
                            {
                                spritePicker += num;
                            }
                        }
                    }
                    num = num * 2;
                }
            }
        }
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        currentSpriteIndex = spritePicker;

        if (tilename == "Bridge")
        {
            switch (spritePicker)
            {
                case 0:
                    this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                    break;
                case 2:
                    this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                    break;
                case 4:
                    this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                    break;
                default:
                    this.GetComponent<SpriteRenderer>().sprite = sprites[3];
                    break;
            }
        }
        else
        {
            switch (spritePicker)
            {
                case 12:
                    renderer.sprite = sprites[0];
                    break;
                case 14:
                    renderer.sprite = sprites[1];
                    break;
                case 10:
                    renderer.sprite = sprites[2];
                    break;
                case 8:
                    renderer.sprite = sprites[3];
                    break;
                case 13:
                    renderer.sprite = sprites[4];
                    break;
                case 15:
                    renderer.sprite = sprites[5];
                    break;
                case 11:
                    renderer.sprite = sprites[6];
                    break;
                case 9:
                    renderer.sprite = sprites[7];
                    break;
                case 5:
                    renderer.sprite = sprites[8];
                    break;
                case 7:
                    renderer.sprite = sprites[9];
                    break;
                case 3:
                    renderer.sprite = sprites[10];
                    break;
                case 1:
                    renderer.sprite = sprites[11];
                    break;
                case 4:
                    renderer.sprite = sprites[12];
                    break;
                case 6:
                    renderer.sprite = sprites[13];
                    break;
                case 2:
                    renderer.sprite = sprites[14];
                    break;
                default:
                    renderer.sprite = sprites[15];
                    break;
            }
        }
    }

    public void SimplePick(int currentIndex)
    {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();

        switch (currentSpriteIndex)
        {
            case 12:
                renderer.sprite = sprites[0 + 16 * currentIndex];
                break;
            case 14:
                renderer.sprite = sprites[1 + 16 * currentIndex];
                break;
            case 10:
                renderer.sprite = sprites[2 + 16 * currentIndex];
                break;
            case 8:
                renderer.sprite = sprites[3 + 16 * currentIndex];
                break;
            case 13:
                renderer.sprite = sprites[4 + 16 * currentIndex];
                break;
            case 15:
                renderer.sprite = sprites[5 + 16 * currentIndex];
                break;
            case 11:
                renderer.sprite = sprites[6 + 16 * currentIndex];
                break;
            case 9:
                renderer.sprite = sprites[7 + 16 * currentIndex];
                break;
            case 5:
                renderer.sprite = sprites[8 + 16 * currentIndex];
                break;
            case 7:
                renderer.sprite = sprites[9 + 16 * currentIndex];
                break;
            case 3:
                renderer.sprite = sprites[10 + 16 * currentIndex];
                break;
            case 1:
                renderer.sprite = sprites[11 + 16 * currentIndex];
                break;
            case 4:
                renderer.sprite = sprites[12 + 16 * currentIndex];
                break;
            case 6:
                renderer.sprite = sprites[13 + 16 * currentIndex];
                break;
            case 2:
                renderer.sprite = sprites[14 + 16 * currentIndex];
                break;
            default:
                renderer.sprite = sprites[15 + 16 * currentIndex];
                break;
        }
    }
}
