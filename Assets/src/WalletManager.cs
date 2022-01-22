using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;

public class WalletManager : MonoBehaviour
{
    public GameObject m_loadWalletUI;
    public WalletConnect m_walletConnect;
    public GetWalletAssets.ReturnData returnData;
    
    public string chain = "ethereum";
    public string network = "optimistic-kovan";
    public string contract = "0x20af0c7dd43fc91e7e8f449692f26adc2fa69ee4";
    public string optimsiticKovanRPC = "https://kovan.optimism.io";

    public string m_loadedWallet = String.Empty;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_walletConnect)
        {
            Debug.LogError("WalletManager: No WalletConnect object given. Wallet Manager will be disabled");
            enabled = false;
            return;
        }
        
        m_walletConnect.ConnectedEvent.AddListener(async delegate
        {
            await WalletConnectedEventHandler();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async Task OnDisable()
    {
        m_walletConnect.createNewSessionOnSessionDisconnect = false;
        await WalletConnect.ActiveSession.Disconnect();
    }

    private async Task WalletConnectedEventHandler()
    {
        Debug.Log("Logged In Wallet: " + WalletConnect.ActiveSession.Accounts[0].ToLower());
        m_loadedWallet = WalletConnect.ActiveSession.Accounts[0].ToLower();
        m_loadWalletUI.SetActive(false);

        // Todo: Make Query Here after user log-in
        returnData = await GetWalletAssets.Fetch(m_loadedWallet);

        if (await VerfiyAssets()) {
            GetAssetUris();
        }
    }

    private async Task<bool> VerfiyAssets() {
        foreach(var assetBalance in returnData.data.user.ownerAssets) {
            BigInteger balance = await ERC1155.BalanceOf(chain, network, contract, m_loadedWallet, assetBalance.asset.tokenId.ToString(), optimsiticKovanRPC);
            Debug.Log("Balance: " + balance);
            if (assetBalance.amount != balance) {
                Debug.LogError("Error: Asset Balance is not the same!");
                return false;
            }
        }
        return true;
    }
    

    private async Task GetAssetUris() {
        foreach(var assetBalance in returnData.data.user.ownerAssets) {
            string uri = await ERC1155.URI(chain, network, contract, assetBalance.asset.tokenId.ToString(), optimsiticKovanRPC);
            // string uri = await ContentManager.TokenUri(chain, network, contract, assetBalance.asset.tokenId.ToString(), optimsiticKovanRPC);
            // string uri = await ContentManager.TokenUriWithVersion(chain, network, contract, assetBalance.asset.tokenId.ToString(), "0", optimsiticKovanRPC);
            Debug.Log("URI: " + uri);
        }
    }
}
