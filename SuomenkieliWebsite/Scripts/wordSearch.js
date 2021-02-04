///<reference path="typings/jquery/jquery.d.ts" />
///<reference path="typings/jqueryui/jqueryui.d.ts" />
var WordInput = (function () {
    function WordInput(wordInputID, onEnterFunc) {
        if (onEnterFunc === void 0) { onEnterFunc = null; }
        WordInput.instance = this;
        this.input = document.getElementById(wordInputID);
        $(this.input).on("change paste keyup", this.onWordChange);
        $(this.input).on("keypress", this.onKeyPress);
        $(this.input).autocomplete({
            minLength: 2,
            source: this.getSuggestions,
            autoFocus: false
        });
        if (onEnterFunc == null)
            this.onEnter = this.onDefaultEnter;
        else
            this.onEnter = onEnterFunc;
        $(this.input).on("autocompleteselect", this.onEnter);
    }
    WordInput.prototype.onWordChange = function (e) {
        //console.log("onWordChange", e);
    };
    WordInput.prototype.onDefaultEnter = function (e) {
        console.log("onEnter: ", e.target);
        var wordInputValue = $(e.target).val();
        if (wordInputValue == "")
            return;
        var ajaxSettings = {};
        ajaxSettings.url = "/api/Word/" + wordInputValue;
        ajaxSettings.context = document;
        ajaxSettings.contentType = "json";
        ajaxSettings.success = WordInput.instance.onDataReturn;
        $.ajax(ajaxSettings);
        $(this.input).autocomplete("close");
    };
    WordInput.prototype.onDataReturn = function (e) {
        console.log("onDataReturn: ", this);
        if (e == null)
            e = "";
        var output = document.getElementById("wordOutput");
        output.innerText = e;
        $(this.input).autocomplete("close");
    };
    WordInput.prototype.onKeyPress = function (e) {
        if (e.which == 13)
            WordInput.instance.onEnter(e);
    };
    WordInput.prototype.getSuggestions = function (request, response) {
        jQuery.get("/api/Search/" + request.term, {}, function (data) {
            response(data);
        });
    };
    return WordInput;
}());
//# sourceMappingURL=wordSearch.js.map