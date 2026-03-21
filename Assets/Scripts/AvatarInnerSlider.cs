using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvatarInnerSlider : MonoBehaviour
{
    [SerializeField] Image image;

    PlayerData playerData;

    public void SetImage(Sprite _sprite)
    {
        image.sprite = _sprite;
    }

    public void SelectAvatar()
    {
        if (UIManager.main != null)
        {
            UIManager.main.SetUnpauseGame();
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.avatar = image.sprite.name;
            PlayerPrefs.SetString("Avatar", image.sprite.name);
            JsonSave.SaveData(playerData, "playerData");

            UIManager.main.GetCurrentAvatar().sprite = image.sprite;
            StartCoroutine(CloseWindow());
        }
    }

    IEnumerator CloseWindow()
    {
        yield return new WaitForSeconds(0.2f);
        UIManager.main.GetSelectAvatarWindow().SetActive(false);
    }
}
