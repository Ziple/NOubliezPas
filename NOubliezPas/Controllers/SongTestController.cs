﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gtk;

using NOubliezPas.Communication;

namespace NOubliezPas.Controllers
{
    class SongTestController: Controller
    {
        VBox vBox = null;
        HBox hBox = null;
        Label answerLabel = null;
        Entry answerEntry = null;
        Button analyzeBtn = null;

        HBox currentAnalyzedWords = null;
        List<Button> currentAnalyzedWordsButtons = null;

        public SongTestController( GUILauncher guiLauncher ):
            base(guiLauncher)
        {
            //Create the main vertical box
            vBox = new VBox();

            hBox = new HBox();
            vBox.Add(hBox);

            // Ajout du label
            answerLabel = new Label("Réponse: ");
            hBox.Add(answerLabel);

            // Ajout de la ligne où entrer le texte
            answerEntry = new Entry();
            answerEntry.TextInserted += this.OnAnswerTextEntered;
            answerEntry.TextDeleted += this.OnAnswerTextDeleted;
            hBox.Add(answerEntry);

            // Ajout du bouton de validation
            analyzeBtn = new Button("Analyser la réponse");
            analyzeBtn.Clicked += this.OnAnalyzeButtonClicked;

            hBox.Add(analyzeBtn);

            // By default, nothing is sensitive
            answerEntry.Sensitive = false;
            analyzeBtn.Sensitive = false;
        }

        public override Box GetPaneBox()
        {
            return vBox;
        }

        public override void ReadMessage( GameToControllerWindowMessage msg )
        {
            if (msg == GameToControllerWindowMessage.ApplicationWaitingAnswer)
                OnWaitingAnswer();
        }

        public void OnWaitingAnswer()
        {
            answerEntry.Sensitive = true;
        }

        public void OnAnswerTextDeleted( object o, TextDeletedArgs a )
        {
            List<string> l = GetWordsList();
            analyzeBtn.Sensitive = (l.Count > 0);

            ClearWordsButtonList();
        }

        public void OnAnswerTextEntered( object o, TextInsertedArgs a)
        {
            List<string> l = GetWordsList();
            analyzeBtn.Sensitive = (l.Count > 0);

            ClearWordsButtonList();
        }

        public void ClearWordsButtonList()
        {
            if (currentAnalyzedWords != null)
            {
                vBox.Remove(currentAnalyzedWords);
                currentAnalyzedWords = null;
            }

            if( currentAnalyzedWordsButtons != null )
            {
                currentAnalyzedWordsButtons.Clear();
                currentAnalyzedWordsButtons = null;
            }
        }

        public List<string> GetWordsList()
        {
            string[] ar = answerEntry.Text.Split(' ');

            List<string> ret = new List<string>();

            foreach (string s in ar)
                if (s != "")
                    ret.Add(s);

            return ret;
        }

        public void OnAnalyzeButtonClicked( object o, EventArgs a)
        {
            ClearWordsButtonList();

            List<string> analyzedWords = GetWordsList();
            if( analyzedWords.Count > 0 )
            {
                currentAnalyzedWords = new HBox();
                currentAnalyzedWordsButtons = new List<Button>();

                Label validerLabel = new Label("Valider jusqu'à:");
                currentAnalyzedWords.Add(validerLabel);

                foreach( string s in analyzedWords )
                {
                    Button sBtn = new Button(s);
                    sBtn.Clicked += this.OnWordClicked;

                    currentAnalyzedWords.Add(sBtn);
                    currentAnalyzedWordsButtons.Add(sBtn);
                }

                currentAnalyzedWords.ShowAll();

                vBox.Add(currentAnalyzedWords);

                analyzeBtn.Sensitive = false;
            }
        }

        public void OnWordClicked( object o, EventArgs a )
        {
            if( currentAnalyzedWordsButtons != null )
            {
                int index = currentAnalyzedWordsButtons.FindIndex(0, currentAnalyzedWordsButtons.Count,
                    delegate(Button b)
                    {
                        return b == (Button)o;
                    }
                    );

                if( index >= 0 )
                {
                    for( int i = 0; i <= index; i++ )
                    {
                        currentAnalyzedWordsButtons[i].Sensitive = false;
                        // envoyer l'événement au jeu
                    }
                }
            }
        }
    }
}