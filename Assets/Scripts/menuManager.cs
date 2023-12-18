using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;


public class menuManager : MonoBehaviour
{
    [SerializeField] string levelName;
    [SerializeField] GameObject UICanvas;
    [SerializeField] GameObject MenuCamera;


    private void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "client") { StartClient(); }
            else if (args[i] == "server") { StartServer(); }
            else if (args[i] == "host") { StartHost(); }
        }
    }

    public void StartHost()
    {
        

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host Conectado");
            serverData();
        }
        else
        {
            Debug.Log("Error al conectar Host");
        }
        if (levelName != string.Empty)
        {
            SceneManager.LoadScene(levelName);
        }
        
    }

    public void StartServer()
    {
        

        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Servidor Conectado");
            serverData();

        }
        else
        {
            Debug.Log("Error al conectar Servidor");
        }
        if (levelName != string.Empty)
        {
            SceneManager.LoadScene(levelName);
        }
    }

    public void StartClient()
    {
        

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Cliente Conectad");
            serverData();
        }
        else
        {
            Debug.Log("Error al conectar Cliente");
        }
        if (levelName != string.Empty)
        {
            SceneManager.LoadScene(levelName);
        }
    }

    private void serverData()
    {
        /*Debug.Log("Address: " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address);
        Debug.Log("Port: " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
        Debug.Log("ServerListenAddress: " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.ServerListenAddress);*/

        Destroy(MenuCamera);
        Destroy(UICanvas);
    }
}
