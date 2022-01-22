using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using Rawrshak;

public class GetWalletAssets : QueryBase
{
    public static async Task<ReturnData> Fetch(string walletAddress) {
        // Load the query
        string query = LoadQuery("GetWalletAssets");

        // Pass in function parameters
        string queryWithParams = String.Format(query, walletAddress.ToLower());

        // Post Query 
        string returnData = await PostAsync("https://api.thegraph.com/subgraphs/name/rawrshak-dev/demo-subgraph2", queryWithParams);

        return JsonUtility.FromJson<ReturnData>(returnData);
    }

// Return Data will look something like this:
// {
//   "data": {
//     "user": {
//       "ownerAssets": [
//         {
//           "amount": "15997",
//           "asset": {
//             "tokenId": "0x0"
//           }
//         },
//       ],
//       "id": "0xb796bce3db9a9dfb3f435a375f69f43a104b4caf"
//     }
//   }
// }

    
    [Serializable]
    public class ReturnData
    {
        public DataObject data;
    }

    [Serializable]
    public class DataObject
    {
        public User user;
    }

    [Serializable]
    public class User
    {
        public string id;
        public AssetBalance[] ownerAssets;
    }

    [Serializable]
    public class AssetBalance
    {
        public int amount;
        public Asset asset;
    }

    [Serializable]
    public class Asset
    {
        public int tokenId;
    }
}
