using ChainSafe.Gaming.UnityPackage;
#if UNITY_WEBGL && !UNITY_EDITOR
using ChainSafe.Gaming.MetaMask;
using ChainSafe.Gaming.MetaMask.Unity;

#endif
using ChainSafe.Gaming.Web3.Build;
using Scenes;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using System.Collections;
using Scripts.EVM.Token;
using ChainSafe.Gaming.Web3;
using Nethereum.Web3;
using TMPro;
using ChainSafe.Gaming;
using System.Numerics;
using Scripts.EVM.Token;
using System.Globalization;


public class PlayerInfo : NetworkBehaviour
{
    
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private GameObject tradeCanvas;
    [SerializeField] private TextMeshProUGUI labelTarget;
    [SerializeField] private Button transferButton;
    [SerializeField] private GameObject inputAmmount;

    private GameObject[] allPlayers;
    private NetworkVariable<FixedString512Bytes> userWallet = new NetworkVariable<FixedString512Bytes>();
    private bool isNameAssigned = false;
    private GameObject tradeableCharacter;


    private async void Start() {
        if(!IsOwner) return;
        gameObject.tag = "Player";

        try
        {
            string walletAddress = await Web3Accessor.Web3.Signer.GetAddress();
            UpdateWalletNameServerRPC(walletAddress);
        }
        catch (System.Exception e)
        {
            print(e);
        }
        
    }

    public async void executeTranfer(){
        if(!IsOwner) return;
        string contractAddress = "0x358969310231363CBEcFEFe47323139569D8a88b";
        string abi = ABI.Erc20;
        
        var contract = Web3Accessor.Web3.ContractBuilder.Build(abi, contractAddress);
        var method = EthMethod.Transfer;
        var toAccount = tradeableCharacter.GetComponent<PlayerInfo>().userWallet.Value.ToString();
        string textfield = inputAmmount.GetComponent<TMP_InputField>().text;
        float amount = float.Parse(textfield) * 1000000000000000000;
        BigInteger amountBI = BigInteger.Parse(amount.ToString("F0"));
        string amountStr = amount.ToString("F0");
        print("Ammount on input: " + textfield);

        print("Ammount to send: " + amount.ToString("F0"));
        print("Ammount str: " + amountStr);

        var response = await Erc20.TransferErc20(Web3Accessor.Web3, Contracts.Erc20, toAccount, amountBI);
        /*var response = await contract.Send(method, new object[]
        {
            toAccount,
            amountStr
        });*/
        print(response);

        tradeCanvas.SetActive(false);
    }

    public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            userWallet.OnValueChanged += (_, newPlayerName) =>
            {
                label.text = newPlayerName.ToString();
            };
        }

    [ServerRpc] private void UpdateWalletNameServerRPC (string _wallet){
        userWallet.Value = _wallet;
        //UpdateWalletNameClientRPC();
    }

    /*[ClientRpc] private void UpdateWalletNameClientRPC(){
        label.text = userWallet.Value.ToString();
    }*/
    private void Update() {
        if (!isNameAssigned && !string.IsNullOrEmpty(label.text))
        {
            label.text = userWallet.Value.ToString();
            isNameAssigned =true;
        }

        if(IsClient && IsOwner){ 
            allPlayers = GameObject.FindGameObjectsWithTag("OtherPlayer");

            foreach (var player in allPlayers)
            {
                if (UnityEngine.Vector3.Distance(gameObject.transform.position, player.transform.position) <= 3)
                {
                    //print("Player at: " + Vector3.Distance(gameObject.transform.position, player.transform.position)); 
                    tradeableCharacter = player;
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                if(UnityEngine.Vector3.Distance(gameObject.transform.position, tradeableCharacter.transform.position) <= 3){
                    print("Wallet: " + tradeableCharacter.GetComponent<PlayerInfo>().userWallet.Value.ToString());
                    tradeCanvas.SetActive(true);
                    labelTarget.text = tradeableCharacter.GetComponent<PlayerInfo>().userWallet.Value.ToString();
                }
            }
        }

        
    }


}
