using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlideshowController : MonoBehaviour
{
    [System.Serializable]
    public class SlideData
    {
        public Sprite sprite;
        public GameObject[] activateOnSlide;
    }

    [Header("Slides")]
    [SerializeField] private Image displayImage;
    [SerializeField] private List<SlideData> slides = new List<SlideData>();

    [Header("Playback")]
    [SerializeField] private float slideDuration = 4f;
    [SerializeField] private bool autoPlay = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onFinished;

    private int currentIndex = -1;
    private float timer;
    private bool playing;

    private void OnDisable()
    {
        StopSequence();
    }

    private void Update()
    {
        if (!playing || !autoPlay || slides.Count == 0)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= slideDuration)
        {
            NextSlide();
        }
    }

    public void StartSequence()
    {
        if (slides.Count == 0)
        {
            Finish();
            return;
        }

        playing = true;
        timer = 0f;
        SetSlide(0);
    }

    public void StopSequence()
    {
        playing = false;
        timer = 0f;
        SetSlide(-1);
    }

    public void NextSlide()
    {
        if (slides.Count == 0)
        {
            return;
        }

        int next = currentIndex + 1;
        if (next >= slides.Count)
        {
            Finish();
            return;
        }

        SetSlide(next);
    }

    public void PreviousSlide()
    {
        if (slides.Count == 0)
        {
            return;
        }

        int prev = Mathf.Max(0, currentIndex - 1);
        SetSlide(prev);
    }

    public void Skip()
    {
        Finish();
    }

    private void Finish()
    {
        playing = false;
        timer = 0f;
        onFinished?.Invoke();
    }

    private void SetSlide(int index)
    {
        DisableCurrentSlideObjects();
        currentIndex = index;
        timer = 0f;

        if (index < 0 || index >= slides.Count)
        {
            if (displayImage != null)
            {
                displayImage.enabled = false;
            }
            return;
        }

        SlideData data = slides[index];
        if (displayImage != null)
        {
            displayImage.enabled = data.sprite != null;
            displayImage.sprite = data.sprite;
        }

        if (data.activateOnSlide != null)
        {
            for (int i = 0; i < data.activateOnSlide.Length; i++)
            {
                if (data.activateOnSlide[i] != null)
                {
                    data.activateOnSlide[i].SetActive(true);
                }
            }
        }
    }

    private void DisableCurrentSlideObjects()
    {
        if (currentIndex < 0 || currentIndex >= slides.Count)
        {
            return;
        }

        SlideData data = slides[currentIndex];
        if (data.activateOnSlide == null)
        {
            return;
        }

        for (int i = 0; i < data.activateOnSlide.Length; i++)
        {
            if (data.activateOnSlide[i] != null)
            {
                data.activateOnSlide[i].SetActive(false);
            }
        }
    }
}
