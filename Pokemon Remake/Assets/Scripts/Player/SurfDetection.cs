﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SurfDetection : MonoBehaviour, Interactable
{
    [SerializeField] GameObject decisionBox;
    [SerializeField] Button firstButton;
    [SerializeField] Decision decision;
    [SerializeField] GameObject player;
    [SerializeField] Dialog myDialog;
   
    public void Interact(Transform initiator)
    {
        string text = "";
        string spritename = player.GetComponent<SpriteRenderer>().sprite.name;
        if (spritename.Contains("Trainer"))
            text = GetSurfablePokemon();
        else
            text = "Would you like to go back on land?";

        myDialog.Lines.Clear();
        myDialog.Lines.Insert(0,text);
        StartCoroutine(DialogManager.Instance.ShowDialog(myDialog));

        // Show decision box if player has surfable pokemon
        if (!text.Equals("You cannot travel on water right now."))
            ShowDecision();
    }

    public string GetSurfablePokemon()
    {
        List<Pokemon> party = player.GetComponent<PokemonParty>().Pokemons;
        var surfPokemon = party.Find(x => x.Base.Name.Equals("Lapras") || x.Base.Name.Equals("Sharpedo"));
        string text = (surfPokemon != null) ? $"Would you like to use " + surfPokemon.Base.Name + " to travel on water?" : "You cannot travel on water right now.";
        return text;
    }

    public void ShowDecision()
    {
        decisionBox.SetActive(true);
        firstButton.Select();
    }

    public void Update()
    {
        if (decision.decision == "yes")
        {
            var x = player.GetComponent<Animator>().GetFloat("MoveX") * 2;
            var y = player.GetComponent<Animator>().GetFloat("MoveY") * 2;
            Vector3 destination = new Vector3(player.transform.position.x + x, player.transform.position.y + y, player.transform.position.z);
            StartCoroutine(MovePlayer(destination));
            decision.decision = null;

        }

        if (decision.decision == "no")
        {
            Debug.Log("Player chose noooo");
            decisionBox.SetActive(false);
            decision.decision = null;

        }
    }

    public IEnumerator MovePlayer(Vector3 targetPosition)
    {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        while ((targetPosition - player.transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, 10 * Time.deltaTime);
            yield return null;
        }
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        player.transform.position = targetPosition;
        var riding = player.GetComponent<Animator>().GetBool("isSurfing");
        player.GetComponent<Animator>().SetBool("isSurfing", !riding);
        decisionBox.SetActive(false);
    }
}