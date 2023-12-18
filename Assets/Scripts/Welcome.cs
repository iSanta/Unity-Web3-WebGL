using ChainSafe.Gaming.UnityPackage;
using TMPro;
using UnityEngine;

//[RequireComponent(typeof(TextMeshProUGUI))]
public class Welcome : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private async void Awake() {
        try
        {
            string walletAddress = await Web3Accessor.Web3.Signer.GetAddress(); 
            label.text = "Welcome " + walletAddress;
        }
        catch (System.Exception e)
        {
            print(e);
        }
        
    }

}







