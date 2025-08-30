using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class CallToAction : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] List<VideoClip> videos = new List<VideoClip>();
    Coroutine videoRoutine;
    private void OnEnable()
    {
        if (videoRoutine != null)
        {
            StopCoroutine(videoRoutine);
        }
        videoRoutine = StartCoroutine(VideoCallback());
    }

    private void OnDisable()
    {
        videoRoutine = null;
    }

    public void CloseWindow()
    {
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        GameManager.gameManager.OpenCourtain(true);
        yield return new WaitForSeconds(.5f);
        GameManager.gameManager.OpenCourtain(false);
        GameManager.gameManager.NextWindow();
    }

    IEnumerator VideoCallback()
    {
        while (true)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < videos.Count; i++)
            {
                index.Add(i);
            }
            List<int> randomList = index.OrderBy(x=>Random.Range(0,1000)).ToList();
            for (int i = 0; i < randomList.Count; i++)
            {
                videoPlayer.clip = videos[randomList[i]];
                videoPlayer.Play();
                yield return new WaitForSeconds((float)videos[randomList[i]].length);
            }
            yield return null;
        }
    }
}
