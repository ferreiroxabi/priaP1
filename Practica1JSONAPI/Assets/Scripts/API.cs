using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class API : MonoBehaviour
{
     [System.Serializable]
    public class TriviaResponse
    {
        public int response_code;
        public List<Question> results;
    }

    [System.Serializable]
    public class Question
    {
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;
    }

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI txtQuestion; // Cambiado a TextMeshPro
    [SerializeField] TextMeshProUGUI txtFeedback; // Cambiado a TextMeshPro
    [SerializeField] Button[] answerButtons;

    private TriviaResponse triviaData;
    private Question currentQuestion;
    private List<string> currentAnswers;
    private int currentQuestionIndex = 0;

    void Start()
    {
        StartCoroutine(LoadQuestions());
    }

    // El resto del script se mantiene igual, solo cambiando los accesos a texto
    IEnumerator LoadQuestions()
    {
        string url = "https://opentdb.com/api.php?amount=10";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                triviaData = JsonUtility.FromJson<TriviaResponse>(request.downloadHandler.text);
                SetupQuestion();
            }
        }
    }

    void SetupQuestion()
    {
       // Verificar si hay más preguntas
    if (currentQuestionIndex >= triviaData.results.Count)
    {
        Debug.Log("No hay más preguntas");
        return;
    }

    // Obtener y limpiar la pregunta actual
    currentQuestion = triviaData.results[currentQuestionIndex];
    string decodedQuestion = UnityWebRequest.UnEscapeURL(currentQuestion.question);
    txtQuestion.text = decodedQuestion;

    // Preparar y mezclar respuestas
    currentAnswers = new List<string>(currentQuestion.incorrect_answers);
    currentAnswers.Add(currentQuestion.correct_answer);
    Shuffle(currentAnswers);

    // Asignar texto a los botones
    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (i >= currentAnswers.Count)
        {
            Debug.LogError("No hay suficientes respuestas para los botones");
            return;
        }

        // Obtener componentes
        Button btn = answerButtons[i];
        TMP_Text buttonText = btn.GetComponentInChildren<TMP_Text>();
        
        // Limpiar y asignar texto
        string decodedAnswer = UnityWebRequest.UnEscapeURL(currentAnswers[i]);
        buttonText.text = decodedAnswer;

        // Configurar evento click
        btn.onClick.RemoveAllListeners();
        bool isCorrect = currentAnswers[i] == currentQuestion.correct_answer;
        btn.onClick.AddListener(() => CheckAnswer(isCorrect));
    }
    }

    void CheckAnswer(bool isCorrect)
    {
        txtFeedback.text = isCorrect ? "¡Correcto!" : "Incorrecto!";
        StartCoroutine(NextQuestion());
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(1.5f);
        currentQuestionIndex++;
        txtFeedback.text = "";
        SetupQuestion();
    }

    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
