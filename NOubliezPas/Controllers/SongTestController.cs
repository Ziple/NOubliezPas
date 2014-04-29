using System;
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
        HBox hBox = null;
        Label answerLabel = null;
        Entry answerEntry = null;
        Button analyzeBtn = null;

        HBox currentAnalyzedWords = null;
        List<Button> currentAnalyzedWordsButtons = null;

        int myNumHolesToFill = -1;

        public SongTestController( GUILauncher guiLauncher ):
            base(guiLauncher)
        {

            hBox = new HBox();
            Add(hBox);

            // Ajout du label
            answerLabel = new Label("Réponse: ");
            hBox.Add(answerLabel);

            // Ajout de la ligne où entrer le texte
            answerEntry = new Entry();
            answerEntry.TextInserted += this.OnAnswerTextEntered;
            answerEntry.TextDeleted += this.OnAnswerTextDeleted;
            hBox.Add(answerEntry);

            // Ajout du bouton de validation
            analyzeBtn = new Button("Mettre la réponse");
            analyzeBtn.Clicked += this.OnAnalyzeButtonClicked;

            hBox.Add(analyzeBtn);

            // By default, nothing is sensitive
            answerEntry.Sensitive = false;
            analyzeBtn.Sensitive = false;

            // show all widgets
            ShowAll();
        }

        public override void ReadMessage( GameToControllerWindowMessage msg )
        {
            if (msg == GameToControllerWindowMessage.ApplicationWaitingAnswer)
                OnWaitingAnswer();
        }

        public void OnWaitingAnswer()
        {
            answerEntry.Sensitive = true;

            Component comp = myGUILauncher.OurGameApp.ActiveComponent;
            SongTest stComp = (SongTest)comp;
            myNumHolesToFill = stComp.GetCurrentSubtitle().NumHoles;
        }

        public void OnAnswerTextDeleted( object o, TextDeletedArgs a )
        {
            List<string> l = GetWordsList();
            analyzeBtn.Sensitive = (l.Count == myNumHolesToFill);

            ClearWordsButtonList();
        }

        public void OnAnswerTextEntered( object o, TextInsertedArgs a)
        {
            List<string> l = GetWordsList();
            analyzeBtn.Sensitive = (l.Count == myNumHolesToFill);

            ClearWordsButtonList();
        }

        public void ClearWordsButtonList()
        {
            if (currentAnalyzedWords != null)
            {
                Remove(currentAnalyzedWords);
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
            if( analyzedWords.Count == myNumHolesToFill )
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

                Add(currentAnalyzedWords);

                analyzeBtn.Sensitive = false;

                SongTest stComp = (SongTest)myGUILauncher.OurGameApp.ActiveComponent;
                stComp.FillHoles(analyzedWords);
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
                    List<bool> validationList = new List<bool>();
                    for( int i = 0; i <= index; i++ )
                    {
                        currentAnalyzedWordsButtons[i].Sensitive = false;
                        // envoyer l'événement au jeu
                        validationList.Add(true);
                    }

                    for( int i = index+1; i < currentAnalyzedWordsButtons.Count; i++ )
                        validationList.Add(false);

                    List<string> analyzedWords = GetWordsList();
                    SongTest stComp = (SongTest)myGUILauncher.OurGameApp.ActiveComponent;
                    stComp.FillHoles(analyzedWords, validationList);
                }
            }
        }
    }
}
