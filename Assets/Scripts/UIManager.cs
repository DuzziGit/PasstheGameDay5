using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour


{

	public GameObject player, ammoSlots;
    public GameObject blackSquare;
    private Player playerScript;
    public Image healthDie;
    public List<Image> gunChambers, gunAmmoLoaded;
    private Color ammoColor;
    public List<int> gunChamberStorage;
    private int ammoValue, ammoSlot;
    public Sprite emptyChamber,YellowSpell,OrangeSpell,PurpleSpell,BlueSpell,RedSpell,GreenSpell;
    public List<Sprite> healthDice;
    private string currentSceneName;

    public TMP_Text gameOver;

    public void Start()
    {
        //playerScript = player.GetComponent<Player>();
        //ammoColor.a = 1;
        ammoSlot = 0;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void HealthChange(int currentHealth)
    {
        if (currentHealth > 0) { 
            //Update Health?
         } else if(currentHealth <= 0) {
			StartCoroutine(FadeBlackOutSquare());
		}
   }


public void RotateBarrel(int currentShot)
{
    gunChambers[currentShot].sprite = emptyChamber;

    // Store the original rotations
    List<Quaternion> originalRotations = new List<Quaternion>();
    foreach (Image chamberImage in gunChambers)
    {
        originalRotations.Add(chamberImage.transform.rotation);
    }

    // Rotate the ammoSlots
    ammoSlots.transform.rotation = Quaternion.Euler(0f, 0f, (currentShot + 1) * 60f);

    // Counter-rotate the gunChambers
    for (int i = 0; i < gunChambers.Count; i++)
    {
        gunChambers[i].transform.rotation = originalRotations[i];
    }
}

    public void AmmoUpdate(List<int> chamberValues)
    {
        gunChamberStorage = chamberValues;
            foreach (int ammoValue in chamberValues)
            {
                switch (ammoValue)
                {
                    case 0:
                        gunChambers[ammoSlot].sprite = BlueSpell;
                       // gunChambers[ammoSlot].color = new Color(1f, 0.13f, 0.27f, 1f);
                        ammoSlot++;
                        break;
                    case 1:
                        gunChambers[ammoSlot].sprite = RedSpell;
                       // gunChambers[ammoSlot].color = new Color(0.97f, 1f, 0.55f, 1f);
                        ammoSlot++;
                        break;
                    case 2:
                        gunChambers[ammoSlot].sprite = OrangeSpell;
                       // gunChambers[ammoSlot].color = new Color(0.39f, 0.25f, 0.84f, 1f);
                        ammoSlot++;
                        break;
                    case 3:
                        gunChambers[ammoSlot].sprite = YellowSpell;
                       // gunChambers[ammoSlot].color = new Color(0.75f, 0.69f, 0.03f, 1f);
                        ammoSlot++;
                        break;
                    case 4:
                        gunChambers[ammoSlot].sprite = PurpleSpell;
                      //  gunChambers[ammoSlot].color = new Color(0.48f, 0.83f, 0.95f, 1f);
                        ammoSlot++;
                        break;
                    case 5:
                        gunChambers[ammoSlot].sprite =GreenSpell;
                       // gunChambers[ammoSlot].color = new Color(0.89f, 0.38f, 0.20f, 1f);
                        ammoSlot++;
                        break;
                    default:
                        break;
                }
        }
        ammoSlot = 0;
    }

    public void Lose()
    {
		StartCoroutine(FadeBlackOutSquare());
	}

	public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed = 1)
	{
		Debug.Log("Fade");
		Color objectColor = blackSquare.GetComponent<Image>().color;
		float fadeAmount;

		if (fadeToBlack)
		{
			while (blackSquare.GetComponent<Image>().color.a < 1)
			{
				fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

				objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
				blackSquare.GetComponent<Image>().color = objectColor;
				yield return null;
			}
		}
		else
		{
			while (blackSquare.GetComponent<Image>().color.a > 0)
			{
				fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

				objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
				blackSquare.GetComponent<Image>().color = objectColor;
				yield return null;
			}
		}
	}
}
