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

	[SerializeField] float rotationSpeed = 0.2f;
	[SerializeField] bool useWorldSpace = true;
	Vector2 _lastFingerPos;
	float _centerYaw;         
	float _currentYawOffset;
	[SerializeField] float maxYaw = 35f;
	private float initialYaw;

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

	public void windowzero()
    {
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}

	public void ShowTargetCar(int carIndex)
    {
        for (int i = 0; i < cars.Count; i++)
        {
			cars[i].HideCar();
        }
		cars[carIndex].ShowCar();
		SetCurrentCar(cars[carIndex]);
		//CarViewManager.ShowCarData(cars[carIndex]);
	}

	float GetYawSigned()
	{
		float y = useWorldSpace ? pivot.eulerAngles.y : pivot.localEulerAngles.y;
		// Llevar a rango [-180, 180]
		if (y > 180f) y -= 360f;
		return y;
	}

	void SetYawSigned(float signedYaw)
	{
		// Normalizar a [0, 360) para asignar en eulerAngles
		float y = signedYaw % 360f;
		if (y < 0f) y += 360f;

		if (useWorldSpace)
		{
			var e = pivot.eulerAngles;
			e.y = y;
			pivot.eulerAngles = e;
		}
		else
		{
			var e = pivot.localEulerAngles;
			e.y = y;
			pivot.localEulerAngles = e;
		}
	}

	float baseYawSigned;


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
		if (pivot != null)
		{
			baseYawSigned = GetYawSigned();
			initialYaw = useWorldSpace ? pivot.eulerAngles.y : pivot.localEulerAngles.y; // si lo usas en otro lado
		}
	}

void OnEnable()
	{
		_centerYaw = useWorldSpace ? pivot.eulerAngles.y : pivot.localEulerAngles.y;

		LeanTouch.OnFingerDown += HandleFingerDown;
		LeanTouch.OnFingerUpdate += HandleFingerUpdate;
	}

	void OnDisable()
	{
		LeanTouch.OnFingerDown -= HandleFingerDown;
		LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
	}


	private ExistingCars currentCar;

	void HandleFingerDown(LeanFinger f)
	{
		if (pivot == null) return;
		if (currentCar != null && currentCar.isImage) return;
		if (f.IsOverGui) return;
		if (LeanTouch.Fingers.Count > 1) return;
	}


	void HandleFingerUpdate(LeanFinger f)
	{
		if (!f.IsActive || pivot == null) return;

		// No rotar si el carro actual es imagen
		if (currentCar != null && currentCar.isImage == true) return;

		if (f.IsOverGui) return;              
		if (LeanTouch.Fingers.Count > 1) return;

		Vector2 delta = f.ScreenDelta;
		if (delta.sqrMagnitude <= 0f) return;

		// Traducimos el drag a variación de yaw (en grados) con tu factor
		float yawDelta = delta.x * rotationSpeed;

		// Yaw actual firmado
		float current = GetYawSigned();

		// Propuesta: sumar delta
		float proposed = current + yawDelta;

		// Limitar contra rango absoluto fijo alrededor de la base
		float minYaw = baseYawSigned - maxYaw;
		float maxYawAbs = baseYawSigned + maxYaw;

		proposed = Mathf.Clamp(proposed, minYaw, maxYawAbs);

		// Aplicar
		SetYawSigned(proposed);
	}


	public void SetCurrentCar(ExistingCars car)
	{
		currentCar = car;


		if (currentCar != null && currentCar.isImage && pivot != null)
		{
			// Para imágenes: fija yaw a la base (o a 0 si así lo prefieres)
			SetYawSigned(baseYawSigned);
			return;
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
		public bool isImage;
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
			GameManager.gameManager.SetCurrentCar(this); // 🔹 ahora asigna el carro
			OnShow?.Invoke(carViews[index]);
		}

		public void Hide()
        {
			foreach (var carView in carViews)
			{
				//carView.virtualCamera.gameObject.SetActive(false);
			}
		}
    }
}
