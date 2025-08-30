using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
	Vector2 lastFingerPos;
	Vector2 direction;
	//[SerializeField] CarViewManager CarViewManager;
	[SerializeField] Transform pivot, popup;
    [SerializeField] List<ExistingCars> cars = new();
	[SerializeField] Animator animator;
	[SerializeField] List<GameObject> windows = new();
	int currentWindow = 0;
	public static GameManager gameManager;

	[Header("Popup")]
	public TextMeshProUGUI title;
	public TextMeshProUGUI description, secondDescription;

    private void Awake()
    {
		gameManager = this;
    }

	public int valueMazda, valueChevrolet, valueToyotaHilux;
	public Slider mazdaSlider, chevroletSlider, toyotaSlider;

	public void SetCar1Value(float a)
    {
		valueMazda = (int)a;
		mazdaSlider.value = valueMazda;
    }

	public void SetCar2Value(float a)
	{
		valueChevrolet = (int)a;
		chevroletSlider.value = valueChevrolet;
	}

	public void SetCar3Value(float a)
	{
		valueToyotaHilux = (int)a;
		toyotaSlider.value = valueToyotaHilux;
	}

	public void OpenCourtain(bool value)
    {
		animator.SetBool("IsClose",value);
    }

	public void NextWindow()
    {
		windows[currentWindow].SetActive(false);
		currentWindow = currentWindow < windows.Count - 1 ? currentWindow + 1 : 0;
        if (currentWindow == 2)
        {
			ShowTargetCar(0);
			/*CarViewManager.StartCountDown(() =>
            {
				NextWindow();
            },60);*/
        }
        if (currentWindow == 0)
        {
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
		windows[currentWindow].SetActive(true);
    }

	public void ShowTargetCar(int carIndex)
    {
        for (int i = 0; i < cars.Count; i++)
        {
			cars[i].HideCar();
        }
		cars[carIndex].ShowCar();
		//CarViewManager.ShowCarData(cars[carIndex]);
    }

    private void Start()
    {
        foreach (var car in cars)
        {
			car.Init();
			car.OnShow += (a)=> {
				title.text = a.title;
				description.text = a.description;
				secondDescription.text = a.secondDescription;
				popup.gameObject.SetActive(true);
				};
		}
		windows[0].SetActive(true);
    }

    void OnEnable()
	{
		LeanTouch.OnFingerUpdate += HandleFingerUpdate;
	}

	void OnDisable()
	{
		LeanTouch.OnFingerUpdate -= HandleFingerUpdate;

		LeanTouch.OnFingerDown += (a) => { Debug.Log("Dafuq"); };
	}

	void HandleFingerUpdate(LeanFinger finger)
	{
		if (finger.IsActive)
        {
			direction.x = (finger.ScreenPosition.x - lastFingerPos.x);
			direction.y = (finger.ScreenPosition.y - lastFingerPos.y);
			lastFingerPos = finger.ScreenPosition;
			pivot.Rotate(Vector3.up, 180 * direction.normalized.x * Time.deltaTime);
        }
	}

	public void HideAll()
    {
        foreach (var car in cars)
        {
			car.Hide();
        }
		popup.gameObject.SetActive(false);
    }

    [System.Serializable]	
	public class CarView
    {
		public CinemachineVirtualCamera virtualCamera;
		public Button button;
		public string title, description, secondDescription;
    }

    [System.Serializable]
	public class ExistingCars
    {
		public string Name, Owners, Kilometers;
		[SerializeField] GameObject CarParent,CarModel;
		[SerializeField] List<CarView> carViews = new();
		public System.Action<CarView> OnShow;
		public void Init()
        {
            for (int i = 0; i < carViews.Count; i++)
            {
                carViews[i].button.onClick.RemoveAllListeners();
				int value = i;
                carViews[i].button.onClick.AddListener(() =>
				{
					Show(value);
				});
			}
		}

		public void ShowCar()
        {
			CarParent.SetActive(true);
			CarModel.SetActive(true);
        }

		public void HideCar()
        {
			CarParent.SetActive(false);
			CarModel.SetActive(false);
        }

		public void Show(int index)
        {
			foreach (var carView in carViews)
			{
				carView.virtualCamera.gameObject.SetActive(false);
			}
            carViews[index].virtualCamera.gameObject.SetActive(true);
            OnShow?.Invoke(carViews[index]);
		}

		public void Hide()
        {
			foreach (var carView in carViews)
			{
				carView.virtualCamera.gameObject.SetActive(false);
			}
		}
    }
}
