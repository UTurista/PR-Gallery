/**
 * Created by Vasco on 02/05/2015.
 */
/* ======================================================================================
 * ==================           BUILDING TILES-CONTAINERS      ==========================
 * ======================================================================================
 */
ASSETS_JSON = '';

$(window).ready(function () {
	$('#SubTitle').html('MapGallery')	
    getJSON('json/serverdata.json', function (metaMap) {

		
		
		$("#Container").append('<div id="Map-Tiles"></div>');
        for (var key in metaMap) {
			var mapName = metaMap[key].MapName;
			
			
			//If map is already in gallery
			if($("#Map-"+mapName).length){
				$("#Map-"+mapName).addClass('fc-'+metaMap[key].Team2FriendlyNameShort).addClass('fc-'+metaMap[key].Team1FriendlyNameShort);

			}else{//If map is not yet in gallery
				var tileMarkup = '<div id="Map-'+mapName+ '" class="tile-container size-'+metaMap[key].MapSize + ' fc-'+metaMap[key].Team2FriendlyNameShort +' fc-'+metaMap[key].Team1FriendlyNameShort+'" data="'+key+'" style="background-color:'+metaMap[key].Color+';">';
				tileMarkup += "<div class='tile-card'>";
				tileMarkup += metaMap[key].FriendlyMapName;
				tileMarkup += "</div>";
				tileMarkup += "<img class='mix tile' src='img/maps/" + mapName + "/tile.jpg'>";
				tileMarkup += "<div class='meta-information'>";
				tileMarkup += "";
				tileMarkup += "</div></div>";
				$("#Map-Tiles").append(tileMarkup);
			}
        }
        setContainerWidth();
        ASSETS_JSON = metaMap;
    });
});


/* ======================================================================================
 * ============================           TILES      =================================
 * ======================================================================================
 */

$(window).resize(function () {
    setContainerWidth();
});

function setContainerWidth() {
    $('#Map-Tiles').css('width', 'auto'); //reset
    var windowWidth = $('#Container').width();
    var blockWidth = $('.tile-container').outerWidth(true);
    var maxBoxPerRow = Math.floor(windowWidth / blockWidth);
    $('#Map-Tiles').width(maxBoxPerRow * blockWidth);
}


/* ======================================================================================
 * =========================           On Map Click      ================================
 * ======================================================================================
 */

$(window).ready(function () {


    $('#Container').on('click', '.tile-container', function () {
        var layouts = [];

        var mapName; 
        var friendlyMapName;
        for (var key in ASSETS_JSON) {

            var name = ASSETS_JSON[key].MapName;
            if("Map-"+name!=$(this).attr('id'))
                continue;

            friendlyMapName = ASSETS_JSON[key].FriendlyMapName;
            mapName = ASSETS_JSON[key].MapName;
            var gmd = ASSETS_JSON[key].GameMode;
            var layer = ASSETS_JSON[key].Layer;
            layouts.push([
                gmd, 
                layer, 
                key,  
                ASSETS_JSON[key].Team1FriendlyName,  
                ASSETS_JSON[key].Team2FriendlyName
            ]);
        }




        // $('body').addClass('noscroll');
        // $('#SubTiles').addClass('open');
        $('#Background').css('background-image', 'url("img/maps/' + mapName+ '/background.jpg")');
		$('#Background').addClass('ready');
        $('#Header').removeClass("opaque");
        $('#Container').addClass('hide');
        $('#PRContainer').removeClass('hide');
		$("#Menu-button").addClass("open").addClass("galleryPage");
        // $('#Filter-container').addClass('hide');

        var layoutsHTML = '<ul>';

        var openingLayout = -1;
        for (var key in layouts) {
            if(layouts[key][0]=='gpm_cq' || layouts[key][0]=='gpm_insurgency' && openingLayout == -1)
                openingLayout = key;

            layoutsHTML += '<li data="'+layouts[key][2]+'">' + dictionary(layouts[key][0]) + ' ' + dictionary(layouts[key][1]) + '</li>'
        }
        layoutsHTML += '</ul>';

        openingLayout = (openingLayout==-1) ? 0 : openingLayout;

        //Create FAB button for layouts
        var fabLay = '<div id="Fab-Layouts" onclick="toggleFAB(\'Fab-Layouts\')" class="fab fab-menu">' + layoutsHTML + '</div>';
        
        //Create FAB button for overviews
        var fabOver = '<div id="Fab-Toggle-Overview" onclick="toggleMapOverview()" class="fab fab-menu"></div>';

        $('#Fab-Anchor').html(fabOver + fabLay);

        buildLayout(layouts[openingLayout][2]);

        var child = parseInt(openingLayout) +1;
        $('#Fab-Layouts li:nth-child('+child+')').addClass('selected');

        $('.team-header.teamB').find('.name').html(layouts[openingLayout][3]);
        $('.team-header.teamA').find('.name').html(layouts[openingLayout][4]);
        $('.team-header.teamB').find('.flag').addClass('ancient-factionflag').addClass('ancient-factionflag-' + layouts[openingLayout].BLUFaction);
        $('.team-header.teamA').find('.flag').addClass('ancient-factionflag').addClass('ancient-factionflag-' + layouts[openingLayout].OPFaction);


        $('#Assets-container').css('height', $(window).height() - 50 - 99 - 9);
    });
});

/* ======================================================================================
 * ======================           Back Button      =======================
 * ======================================================================================
 */
 
$(window).ready(function () {


	$('#Header').on('click', '.galleryPage', function () {
		closeFABs();
		$('#Title').html("Project Reality");
        $('#SubTitle').html('MapGallery')
		$('#Background').removeClass('ready');
		$('#Container').removeClass('hide');
        $('#PRContainer').addClass('hide');
		$('#Fab-Anchor').html('');
	});
});



/* ======================================================================================
 * ============================           LAYOUTS      =================================
 * ======================================================================================
 */

$(window).ready(function () {

    $('#Fab-Anchor').on('click', 'li', function () {
		closeFABs();
        $(this).siblings('li').removeClass('selected');
        $(this).addClass('selected');
		
        buildLayout(parseInt($(this).attr('data')), parseInt($(this).prevAll('li').length));

    });
});

/* ======================================================================================
 * =========================           buildLayout      ================================
 * ======================================================================================
 */


function buildLayout(layout) {
    var map = ASSETS_JSON[layout];

	$('#Title').html(map.FriendlyMapName);
	$('#SubTitle').html(dictionary(map.GameMode) + ' ' + dictionary(map.Layer));

  

    $('#MapOverview').css('background-image', "url(img/maps/"+map.MapName+"/mapOverview_"+map.GameMode+"_"+map.Layer+".png)");


    $('.assets.teamA').html('');//RESET
    $('.assets.teamB').html('');//RESET


    var teamB ="";
    var teamA ="";
    var posA = 0;
    var posB = 0;
    for (var key in map.Spawners) {
        var asset =map.Spawners[key];

        if(asset.Team==2){
            teamA += rowAsset(posB++, asset.FriendlyName, asset.Quantity ,asset.MaxSpawnDelay, asset.SpawnDelayAtStart, '');
        }else{
            teamB += rowAsset(posA++, asset.FriendlyName, asset.Quantity ,asset.MaxSpawnDelay, asset.SpawnDelayAtStart, '');
        }
    }
	
	if(teamA=="")
		teamA +=rowEmpty();
	if(teamB=="")
		teamB +=rowEmpty();
	
   $('.assets.teamB').append(teamB);
   $('.assets.teamA').append(teamA);
   
   

   

}

function rowEmpty() {
    var html = '';

    html += '<div class="empty">No vehicles on this layout.</div>'

    return html;
}


function rowAsset(index, name, qt, delay, start, img) {

    if (parseInt(delay) > 9999)
        delay = 'Never';
    else
        delay = Math.round((delay / 60)) + 'm';

    var html = '';
    html += '<div class="asset-row slide-up" style="-webkit-animation-delay: ' + (index * 50) + 'ms; animation-delay:' + (index * 50) + 'ms"> ';
    html += '<div class="asset-img" style="background-image: url(img/assets/mini_heavyhelo.png)"></div>'
    html += '<div class="info">';
    html += '<div>';
    html += '<div class="asset-qt">' + qt + 'x</div>';
    html += '<div class="asset-name">' + name + '</div>';
    html += '</div><div>';
    html += '<div class="asset-delay">Spawn Time: ' + delay + '</div>';
    if(toTitleCase(start+'')=='True')
        html += '<div class="asset-start">Delayed</div>';
    html += '</div></div></div>';
    return html;

}

/* ======================================================================================
 * =========================             Search          ================================
 * ======================================================================================
 */
 
 $(window).ready(function () {
  $('#Search input').on('input', function() { 
    filterMapsByName($(this).val());
  });
});
 
 
function filterMapsByName(nameToFilter){
  $(".tile-container").each(function(){
    var mapName = $(this).find(".tile-card").text();

    if(nameToFilter == "" || mapName.toLowerCase().indexOf(nameToFilter.toLowerCase()) >= 0 )
      $(this).removeClass("hide");
    else
       $(this).addClass("hide");
     
  });
}
 
 
 
 
 
 