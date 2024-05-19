using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestLoadMiniBtn : MonoBehaviour
{
	private int patternIdx;
	private int patternLevel;

	void Start()
	{
		Button btn = this.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);

		patternLevel = int.Parse(this.gameObject.name);
		patternIdx = this.gameObject.transform.parent.GetSiblingIndex();
	}

	void TaskOnClick()
	{
        TestLoadMini.patternIdx = patternIdx;
		TestLoadMini.patternLevel = patternLevel;
		SceneManager.LoadScene("2_Mini");
	}
}
