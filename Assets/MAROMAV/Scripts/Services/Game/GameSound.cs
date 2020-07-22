using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class GameSound : MonoBehaviour
    {
		public AudioClip joinClip;
		public AudioClip leaveClip;

		private AudioSource _audioSource;

		public static GameSound Instance { get; private set; }

		void Awake()
		{
			Instance = this;
		}

		//  룸에 다른 플레이어 참여시 플레이
		public void PlaySomeoneJoinSound()
		{
			PlayOneShot(joinClip);
		}

		//  룸에 있던 플레이어 연결종료시 플레이
		public void PlaySomeoneLeaveSound()
		{
			PlayOneShot(leaveClip);
		}

		public void PlayOneShot(AudioClip clip)
		{
			if (clip != null)
			{
				if (_audioSource == null)
				{
					_audioSource = GetComponent<AudioSource>();
					_audioSource.PlayOneShot(clip);
				}
				else
				{
					_audioSource.PlayOneShot(clip);
				}
			}
		}
    }
}

