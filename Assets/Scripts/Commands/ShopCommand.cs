using System.Collections.Generic;

namespace Commands {

    public class ShopCommand : Command {

        public override string description => $"Opens the shop, displaying the items purchaseable with {GameState.CURRENCY_SYMBOL}. Auto-closes on purchase or on {InputHandler.CANCEL_COMMAND}. Navigate the pages with {InputHandler.SCROLL_COMMAND}.";

        protected override IEnumerable<string> parameterNames => default;

        protected override bool TryExecute (string[] parameters, out string message) {
            MainDisplay.ShowShop();
            message = default;
            return false;
        }

    }

}