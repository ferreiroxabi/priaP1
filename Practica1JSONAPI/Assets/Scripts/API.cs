using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    [System.Serializable] // Necesario para que Unity poida serializar esta clase
    public class TriviaResponse
    {
        public int response_code;
        public List<Question> results;
    }

    [System.Serializable]
    public class Question
    {
        public string category;
        public string type;
        // IMPORTANTE: Os nomes deben coincidir EXACTAMENTE co JSON
        public string difficulty;
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;
    }

    // Variable pública para ver os datos no Inspector
    public TriviaResponse triviaData;

    void Start()
    {
        StartCoroutine(GetTriviaQuestions());
    }

    IEnumerator GetTriviaQuestions()
    {
        string url = "https://opentdb.com/api.php?amount=10";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                triviaData = JsonUtility.FromJson<TriviaResponse>(webRequest.downloadHandler.text);
                PrintResults();
            }
            else
            {
                Debug.LogError("Erro na conexión: " + webRequest.error);
            }
        }
    }

    void PrintResults()
    {
        Debug.Log("Total preguntas: " + triviaData.results.Count);
        
        if (triviaData.results.Count > 0)
        {
            Question primera = triviaData.results[0];
            Debug.Log("Categoría: " + primera.category);
            Debug.Log("Dificultade: " + primera.difficulty);
            Debug.Log("Pregunta: " + primera.question);
            Debug.Log("Resposta correcta: " + primera.correct_answer);
        }
    }
}
