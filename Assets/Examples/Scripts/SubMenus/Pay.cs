using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Example
{
    internal class Pay : MenuBase
    {
        public string PayProduct = "";

        private void CallFBPay()
        {
            FB.Canvas.Pay(PayProduct, callback: handleResult);
        }

        protected override void getGui()
        {
            LabelAndTextField("Product: ", ref PayProduct);
            if (Button("Call Pay"))
            {
                CallFBPay();
            }
            GUILayout.Space(10);
        }
    }
}
