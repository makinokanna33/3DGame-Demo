using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class BattleWinArgs
    {
        public int awardId;
        public List<string> challengeInfo;
        public List<bool> isComplete;
        public SelectLevelDataProxy dataProxy;

        public BattleWinArgs(int awardId, List<string> challengeInfo, List<bool> isComplete, SelectLevelDataProxy dataProxy)
        {
            this.awardId = awardId;
            this.challengeInfo = challengeInfo;
            this.isComplete = isComplete;
            this.dataProxy = dataProxy;
        }
    }
}

