/**
 * Created by Vasco on 02/05/2015.
 */

var DICTIONARY_SMALL = [
    ["gpm_insurgency", "Ins."],
    ["gpm_skirmish", "Skir."],
    ["gpm_cq", "AAS"],
    ["16", "Inf."],
    ["32", "Alt."],
    ["64", "Std."],
    ["128", "Lrg."],
];
var DICTIONARY_LARGE = [
    ["gpm_insurgency", "Insurgency"],
    ["gpm_skirmish", "Skirmish"],
    ["gpm_cq", "AAS"],
    ["gpm_cnc", "Cnc"],
    ["gpm_coop", "Co-Op"],
    ["gpm_vehicles", "Vehi. Warfare"],

    ["16", "Infantry"],
    ["32", "Alternative"],
    ["64", "Standard"],
    ["128", "Large"],


    ["cf", "Canadian Forces"],
    ["ch", "Chinese Forces"],
    ["chinsurgent", "Militia"],
    ["fr", "French Forces"],
    ["gb", "British Armed Forces"],
    ["ger", "German Forces"],
    ["hamas", "Hamas"],
    ["idf", "Israeli Defence Forces"],
    ["mec", "Middle East Coalition"],
    ["meinsurgent", "Iraqi Insurgents"],
    ["ru", "Russian Armed Forces"],
    ["taliban", "Taliban"],
    ["us", "United States Marine Corps"],
    ["usa", "United States Army"],
    ["ww2ger", "Wehrmacht"],
    ["ww2usa", "United States Army"],
    ["vnnva", "North Vietnamese Army"],
    ["vnusa", "Unite dStates Army"],
    ["vnusmc", "United States Marine Corps"],
    ["vnvc", "Viet Cong"],
    ["arg82", "Argentine Armed Forces"],
    ["gb82", "British Armed Forces"],
    ["arf", "African Resistance Fighters"],
    ["fsa", "Syrian Rebels"],


    ["AF", "Afghanistan"],
    ["AX", "ÅlandIslands"],
    ["AL", "Albania"],
    ["DZ", "Algeria"],
    ["AS", "AmericanSamoa"],
    ["AD", "Andorra"],
    ["AO", "Angola"],
    ["AI", "Anguilla"],
    ["AQ", "Antarctica"],
    ["AG", "AntiguaandBarbuda"],
    ["AR", "Argentina"],
    ["AM", "Armenia"],
    ["AW", "Aruba"],
    ["AU", "Australia"],
    ["AT", "Austria"],
    ["AZ", "Azerbaijan"],
    ["BS", "Bahamas"],
    ["BH", "Bahrain"],
    ["BD", "Bangladesh"],
    ["BB", "Barbados"],
    ["BY", "Belarus"],
    ["BE", "Belgium"],
    ["BZ", "Belize"],
    ["BJ", "Benin"],
    ["BM", "Bermuda"],
    ["BT", "Bhutan"],
    ["BO", "Bolivia, PlurinationalStateof"],
    ["BQ", "Bonaire, SintEustatiusandSaba"],
    ["BA", "Bosniaand Herzegovina"],
    ["BW", "Botswana"],
    ["BV", "BouvetIsland"],
    ["BR", "Brazil"],
    ["IO", "British IndianOcean Territory"],
    ["BN", "BruneiDarussalam"],
    ["BG", "Bulgaria"],
    ["BF", "BurkinaFaso"],
    ["BI", "Burundi"],
    ["KH", "Cambodia"],
    ["CM", "Cameroon"],
    ["CA", "Canada"],
    ["CV", "CapeVerde"],
    ["KY", "CaymanIslands"],
    ["CF", "CentralAfricanRepublic"],
    ["TD", "Chad"],
    ["CL", "Chile"],
    ["CN", "China"],
    ["CX", "ChristmasIsland"],
    ["CC", "Cocos(Keeling)Islands"],
    ["CO", "Colombia"],
    ["KM", "Comoros"],
    ["CG", "Congo"],
    ["CD", "Congo],theDemocraticRepublicofthe"],
    ["CK", "CookIslands"],
    ["CR", "CostaRica"],
    ["CI", "Côted'Ivoire"],
    ["HR", "Croatia"],
    ["CU", "Cuba"],
    ["CW", "Curaçao"],
    ["CY", "Cyprus"],
    ["CZ", "CzechRepublic"],
    ["DK", "Denmark"],
    ["DJ", "Djibouti"],
    ["DM", "Dominica"],
    ["DO", "DominicanRepublic"],
    ["EC", "Ecuador"],
    ["EG", "Egypt"],
    ["SV", "ElSalvador"],
    ["GQ", "EquatorialGuinea"],
    ["ER", "Eritrea"],
    ["EE", "Estonia"],
    ["ET", "Ethiopia"],
    ["FK", "FalklandIslands(Malvinas)"],
    ["FO", "FaroeIslands"],
    ["FJ", "Fiji"],
    ["FI", "Finland"],
    ["FR", "France"],
    ["GF", "FrenchGuiana"],
    ["PF", "FrenchPolynesia"],
    ["TF", "FrenchSouthernTerritories"],
    ["GA", "Gabon"],
    ["GM", "Gambia"],
    ["GE", "Georgia"],
    ["DE", "Germany"],
    ["GH", "Ghana"],
    ["GI", "Gibraltar"],
    ["GR", "Greece"],
    ["GL", "Greenland"],
    ["GD", "Grenada"],
    ["GP", "Guadeloupe"],
    ["GU", "Guam"],
    ["GT", "Guatemala"],
    ["GG", "Guernsey"],
    ["GN", "Guinea"],
    ["GW", "Guinea-Bissau"],
    ["GY", "Guyana"],
    ["HT", "Haiti"],
    ["HM", "HeardIslandandMcDonaldIslands"],
    ["VA", "HolySee(VaticanCityState)"],
    ["HN", "Honduras"],
    ["HK", "HongKong"],
    ["HU", "Hungary"],
    ["IS", "Iceland"],
    ["IN", "India"],
    ["ID", "Indonesia"],
    ["IR", "Iran],IslamicRepublicof"],
    ["IQ", "Iraq"],
    ["IE", "Ireland"],
    ["IM", "IsleofMan"],
    ["IL", "Israel"],
    ["IT", "Italy"],
    ["JM", "Jamaica"],
    ["JP", "Japan"],
    ["JE", "Jersey"],
    ["JO", "Jordan"],
    ["KZ", "Kazakhstan"],
    ["KE", "Kenya"],
    ["KI", "Kiribati"],
    ["KP", "Korea],DemocraticPeople'sRepublicof"],
    ["KR", "Korea],Republicof"],
    ["KW", "Kuwait"],
    ["KG", "Kyrgyzstan"],
    ["LA", "LaoPeople'sDemocraticRepublic"],
    ["LV", "Latvia"],
    ["LB", "Lebanon"],
    ["LS", "Lesotho"],
    ["LR", "Liberia"],
    ["LY", "Libya"],
    ["LI", "Liechtenstein"],
    ["LT", "Lithuania"],
    ["LU", "Luxembourg"],
    ["MO", "Macao"],
    ["MK", "Macedonia, TheFormerYugoslav Republic of"],
    ["MG", "Madagascar"],
    ["MW", "Malawi"],
    ["MY", "Malaysia"],
    ["MV", "Maldives"],
    ["ML", "Mali"],
    ["MT", "Malta"],
    ["MH", "MarshallIslands"],
    ["MQ", "Martinique"],
    ["MR", "Mauritania"],
    ["MU", "Mauritius"],
    ["YT", "Mayotte"],
    ["MX", "Mexico"],
    ["FM", "Micronesia, FederatedStatesof"],
    ["MD", "Moldova, Republicof"],
    ["MC", "Monaco"],
    ["MN", "Mongolia"],
    ["ME", "Montenegro"],
    ["MS", "Montserrat"],
    ["MA", "Morocco"],
    ["MZ", "Mozambique"],
    ["MM", "Myanmar"],
    ["NA", "Namibia"],
    ["NR", "Nauru"],
    ["NP", "Nepal"],
    ["NL", "Netherlands"],
    ["NC", "NewCaledonia"],
    ["NZ", "NewZealand"],
    ["NI", "Nicaragua"],
    ["NE", "Niger"],
    ["NG", "Nigeria"],
    ["NU", "Niue"],
    ["NF", "NorfolkIsland"],
    ["MP", "NorthernMarianaIslands"],
    ["NO", "Norway"],
    ["OM", "Oman"],
    ["PK", "Pakistan"],
    ["PW", "Palau"],
    ["PS", "PalestinianTerritory, Occupied"],
    ["PA", "Panama"],
    ["PG", "PapuaNewGuinea"],
    ["PY", "Paraguay"],
    ["PE", "Peru"],
    ["PH", "Philippines"],
    ["PN", "Pitcairn"],
    ["PL", "Poland"],
    ["PT", "Portugal"],
    ["PR", "PuertoRico"],
    ["QA", "Qatar"],
    ["RE", "Réunion"],
    ["RO", "Romania"],
    ["RU", "RussianFederation"],
    ["RW", "Rwanda"],
    ["BL", "SaintBarthélemy"],
    ["SH", "SaintHelena, AscensionandTristandaCunha"],
    ["KN", "SaintKittsandNevis"],
    ["LC", "SaintLucia"],
    ["MF", "SaintMartin(Frenchpart)"],
    ["PM", "SaintPierreandMiquelon"],
    ["VC", "SaintVincentandtheGrenadines"],
    ["WS", "Samoa"],
    ["SM", "SanMarino"],
    ["ST", "SaoTomeandPrincipe"],
    ["SA", "SaudiArabia"],
    ["SN", "Senegal"],
    ["RS", "Serbia"],
    ["SC", "Seychelles"],
    ["SL", "SierraLeone"],
    ["SG", "Singapore"],
    ["SX", "SintMaarten(Dutchpart)"],
    ["SK", "Slovakia"],
    ["SI", "Slovenia"],
    ["SB", "SolomonIslands"],
    ["SO", "Somalia"],
    ["ZA", "SouthAfrica"],
    ["GS", "SouthGeorgiaandtheSouthSandwichIslands"],
    ["SS", "SouthSudan"],
    ["ES", "Spain"],
    ["LK", "SriLanka"],
    ["SD", "Sudan"],
    ["SR", "Suriname"],
    ["SJ", "SvalbardandJanMayen"],
    ["SZ", "Swaziland"],
    ["SE", "Sweden"],
    ["CH", "Switzerland"],
    ["SY", "SyrianArabRepublic"],
    ["TW", "Taiwan],ProvinceofChina"],
    ["TJ", "Tajikistan"],
    ["TZ", "Tanzania],UnitedRepublicof"],
    ["TH", "Thailand"],
    ["TL", "Timor-Leste"],
    ["TG", "Togo"],
    ["TK", "Tokelau"],
    ["TO", "Tonga"],
    ["TT", "TrinidadandTobago"],
    ["TN", "Tunisia"],
    ["TR", "Turkey"],
    ["TM", "Turkmenistan"],
    ["TC", "TurksandCaicosIslands"],
    ["TV", "Tuvalu"],
    ["UG", "Uganda"],
    ["UA", "Ukraine"],
    ["AE", "UnitedArabEmirates"],
    ["GB", "UnitedKingdom"],
    ["US", "UnitedStates"],
    ["UM", "UnitedStatesMinorOutlyingIslands"],
    ["UY", "Uruguay"],
    ["UZ", "Uzbekistan"],
    ["VU", "Vanuatu"],
    ["VE", "Venezuela],BolivarianRepublicof"],
    ["VN", "VietNam"],
    ["VG", "VirginIslands, British"],
    ["VI", "VirginIslands, U.S."],
    ["WF", "WallisandFutuna"],
    ["EH", "WesternSahara"],
    ["YE", "Yemen"],
    ["ZM", "Zambia"],
    ["ZW", "Zimbabwe"]
];


function dictionary(word) {
    word += ""; //Make sure is a string
    for (var key in DICTIONARY_LARGE) {
        if (DICTIONARY_LARGE[key][0].toLowerCase() == word.toLowerCase())
            return DICTIONARY_LARGE[key][1];
    }

    return word;
}

function toTitleCase(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
/*
 * GetJSON(url, calback)
 * - Assumes its a url to a JSON file
 * - Calls a callback with the JSON as parameter
 * */
function getJSON(url, calback) {
    var xmlhttp = new XMLHttpRequest();

    xmlhttp.onreadystatechange = function () {
		console.log(xmlhttp.readyState);
		console.log(xmlhttp.status);
		console.log(">"+xmlhttp.responseText.length);
        if (xmlhttp.readyState == 4 && (xmlhttp.status == 200 || xmlhttp.status == 304)) {
			console.log(">"+xmlhttp.responseText);
            var json = JSON.parse(xmlhttp.responseText);
            calback(json);
        }
    }
    xmlhttp.open("GET", url, true);
    xmlhttp.send();
}

function AJAX(src, calback) {

    $.ajax({
        dataType: "html",
        url: src,
        success: function (data) {
            calback(data);
        }
    });
}


/* ======================================================================================
 * ==================     PRContainer onFaction Click      ==============================
 * ======================================================================================
 */
$(window).ready(function () {
    $('#PRContainer').on('click', '.team-header', function () {
        $(this).siblings('.team-header').removeClass('selected');
        $(this).addClass('selected');
        if ($(this).hasClass("teamA"))
            $("#Assets-container").find('.asset-pane').css("left", "-0%");
        else
            $("#Assets-container").find('.asset-pane').css("left", "-100%");
    });
});


/* ======================================================================================
 * ============================           LEFT MENU      =================================
 * ======================================================================================
 */
$(window).ready(function () {
    $('#Menu-button').click(function () {

        if ($('#Menu-button').hasClass('open')) {
            $("#Body-Wrapper").removeClass("open-menu");
            $("#Menu-button").removeClass("open");

        } else {
            $("#Body-Wrapper").addClass("open-menu");
            $("#Menu-button").addClass("open");

        }
    });
    $("#Nav-Overlay").click(function () {
        $("#Body-Wrapper").removeClass("open-menu");
        $("#Menu-button").removeClass("open");
    });
});


/* ======================================================================================
 * ============================           closeFABs      =================================
 * ======================================================================================
 */
function closeFABs(){
    $('#Fab-Shadow').addClass('hide')
    $('.fab').removeClass('open');
     $("body").removeClass('noScroll');
    $('#MapOverview').addClass('oHide'); 

}

/* ======================================================================================
 * ============================           toggleFAB      =================================
 * ======================================================================================
 */
function toggleFAB(view) {
    $('.fab').removeClass('open');
    var mView = $('#' + view);
    var mShadow = $('#Fab-Shadow');
    if (mView.hasClass('open')) {
        mShadow.addClass('hide')
        mView.removeClass('open');
        
    } else {
		$('#MapOverview').addClass('oHide'); 
        mShadow.removeClass('hide')
        mView.addClass('open');
        $("body").addClass('noScroll');

    }
}

/* ======================================================================================
 * ========================           TOGGLE Image Overview       =======================
 * ======================================================================================
 */



function toggleMapOverview() {
    var mView = $('#MapOverview');


    if (mView.hasClass('oHide')) {
        mView.removeClass('oHide'); 
        $('#Fab-Shadow').removeClass('hide')
        $("body").addClass('noScroll');
    }else{
        mView.addClass('oHide');
        $('#Fab-Shadow').addClass('hide')
        $("body").removeClass('noScroll');
     }
      
}











