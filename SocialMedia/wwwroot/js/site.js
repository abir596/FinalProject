var findBtn = document.getElementById('findButton');
var searchBar = document.getElementById('searchBar');
var closeBtn = document.getElementById('closeBtn');
var searchBtn = document.getElementById('searchButton');
var searchRes = document.getElementById('searchItems');
var finder = document.getElementById('FinderElement');
var searched;

findBtn.addEventListener('click', searchOption);
searchBtn.addEventListener('click', searchResults);

function searchOption() {
    finder.style.display = 'none';
    searchBar.style.display = 'block';
    closeBtn.style.display = 'block';
}
closeBtn.addEventListener('click', closeSearch);

function closeSearch() {
    localStorage.setItem('searched', 'no');
    finder.style.display = 'flex';
    searchBar.style.display = 'none';
    closeBtn.style.display = 'none';
    searchRes.style.display = 'none';
}

function searchResults() {
    localStorage.setItem('searched', 'yes');
    searchRes.style.display = 'block';
    
    finder.style.display = 'none';
    
    searchBar.style.display = 'block';
    
    closeBtn.style.display = 'block';
}
$(document).ready(function () {
    searched = localStorage.getItem('searched');
    if (searched == 'yes') {
        searchRes.style.display = 'block';
        finder.style.display = 'none';
        searchBar.style.display = 'block';
        closeBtn.style.display = 'block';
    }
    localStorage.setItem('searched', 'no');
});
function LoadDiv(url) {
    var img = new Image();
    var bcgDiv = document.getElementById('divBackground');
    var imgDiv = document.getElementById('divImage');
    var imgFull = document.getElementById('imgFull');
    var imgLoader = document.getElementById('imgLoader');
    imgLoader.style.display = "block";
    img.onload = function () {
        imgFull.src = img.src;
        imgFull.style.display = "block";
        imgLoader.style.display = "none";
    };
    img.src = url;
    var width = document.body.clientWidth;
    if (document.body.clientHeight > document.body.scrollHeight) {
        bcgDiv.style.height = document.body.clientHeight + "px";
    }
    else {
        bcgDiv.style.height = document.body.scrollHeight + "px";
    }
    imgDiv.style.left = (width - 650) / 2 + "px";
    imgDiv.style.top = "20px";
    bcgDiv.style.width = "100%";

    bcgDiv.style.display = "block";
    imgDiv.style.display = "block";
    return false;
}
function HideDiv() {
    var bcgDiv = document.getElementById("divBackground");
    var imgDiv = document.getElementById("divImage");
    var imgFull = document.getElementById("imgFull");
    if (bcgDiv != null) {
        bcgDiv.style.display = "none";
        imgDiv.style.display = "none";
        imgFull.style.display = "none";
    }
}