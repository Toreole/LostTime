using System.Collections;
using UnityEngine;

namespace LostTime.Core
{
    public class LevelStartArea : MonoBehaviour
    {
        public static LevelStartArea Current;

        [SerializeField]
        private Transform defaultStartingLocation;
        [SerializeField]
        private Item completionItem;
        [SerializeField]
        Audio.VoiceOver onLevelCompleteVO;

        public Item CompletionItem => completionItem;
        public Audio.VoiceOver OnLevelCompleteVoiceOver => onLevelCompleteVO;

        private void Awake()
        {
            Current = this;
        }

        public void ResetPlayer(Transform playerTransform)
        {
            playerTransform.position = defaultStartingLocation.position;
            playerTransform.rotation = defaultStartingLocation.rotation;
        }

        private void OnDestroy()
        {
            Current = null;
        }
    }
}