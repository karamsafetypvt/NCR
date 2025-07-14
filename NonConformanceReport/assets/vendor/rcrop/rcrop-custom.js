$(document).ready(function () {
    //CROP START============================================================
    function AlertWithTitleIcon(title, text, type) {
        swal({
            title: title,
            text: text,
            type: type
        });
    }


    if ($("#squareCropModal").length > 0)
        $("#squareCropModal").remove();

    var squareContent = '<!-- Modal -->';
    squareContent += ' <div id = "squareCropModal" class="modal fade1 draggable-modal" id="draggable">';
    squareContent += ' <div class="modal-dialog">';
    squareContent += ' <!-- Modal content-->';
    squareContent += ' <div class="modal-content">';
    squareContent += ' <div class="modal-header">';
    squareContent += '  <button type="button" class="close clearPic" data-dismiss="modal">&times;</button>';
    squareContent += '  <h4 class="modal-title"></h4>';
    squareContent += ' </div>';
    squareContent += ' <div class="modal-body" style="max-height:510px; overflow: auto;">';
    squareContent += '  <div class="col-lg-12">';
    squareContent += '  <div class="row">';
    squareContent += ' <div class="custom-file" style="margin-right: 1px;">';
    squareContent += '  <input type="file" id="fileCrop" class="custom-file-input" />';
    squareContent += '<label class="custom-file-label" for="customFile">Choose file</label>';
    squareContent += '  </div></div>';
    squareContent += '  <div class="row">';
    squareContent += '  <div class="image-wrapper">';
    squareContent += '  <img id="imgCropView" >';
    squareContent += ' </div> </div></div></div>';
    squareContent += '  <div class="modal-footer" style="padding: 0 25px 25px;">';
    squareContent += '      <button type="button" class="btn btn-default clearPic" style="margin-top:10px;" data-dismiss="modal">Close</button>';
    squareContent += '      <button type="button" class="btn btn-primary UploadCropImage btn-disable">Save</button>';
    squareContent += '  </div>';
    squareContent += ' </div></div></div>';

    $("body").append(squareContent);



    //var _fileId = "";
    var _cImgOneId = "";
    var _cImgTwoId = "";
    var _cImgThreeId = "";
    var _cHFId = "";
    var _cWidth = 150;
    var _cHeight = 150;
    var _cHeight = 150;
    // var _cBtnSave = "";
    // $(".cropBox").click(function () {
    $("body").on("click", ".cropBox", function () {

        if (!$(".UploadCropImage").hasClass("btn-disable"))
            $(".UploadCropImage").addClass("btn-disable");
        //_fileId = $(this).attr("data");
        _cImgOneId = $(this).attr("cImgOne");
        _cImgTwoId = $(this).attr("cImgTwo");
        _cImgThreeId = $(this).attr("cImgThree");
        _cHFId = $(this).attr("cHF");

        _cWidth = parseInt($(this).attr("cWidth"));
        _cHeight = parseInt($(this).attr("cHeight"));

        // hide save button


        var attr = $(this).attr("cBtnSave");
        // For some browsers, `attr` is undefined; for others,
        // `attr` is false.  Check for both.
        //  debugger;
        if (typeof attr == "undefined" || attr == false) {
            $(".UploadCropImage").css({ display: "none" });
        }

        $("#imgCropView").attr("src", "");
        $("#imgCropView").rcrop('destroy');
        $("#squareCropModal").modal("show");


    });

    $("#fileCrop").change(function () {
        // debugger
        var reader = new FileReader();

        // for image width/height validation
        var _currentPath = "";
        var imageVal = new Image();

        reader.onload = function (e) {
            _currentPath = e.target.result;
            imageVal.src = e.target.result;
        }
        reader.readAsDataURL($(this)[0].files[0]);

        imageVal.onload = function () {
            if (this.width < _cWidth || this.height < _cHeight) {
                $("#fileCrop").val('');
                AlertWithTitleIcon("Warning", "Image size should be " + _cWidth + "x" + _cHeight + " or greater. Current size is " + this.width + "x" + this.height + ".");
                return false;
            }
            else {
                // debugger
                $("#imgCropView").attr("src", _currentPath);

                $('#image-wrapper').empty();
                $("#imgCropView").rcrop('destroy');

                $("#imgCropView").rcrop({
                    minSize: [_cWidth, _cHeight],
                    //maxSize: [250, 250],
                    preserveAspectRatio: true,
                    grid: true,
                    inputs: true,

                    preview: {
                        display: false,
                        size: [100, 100],
                    }
                });

                setTimeout(function () { $('#imgCropView').trigger('rcrop-changed')},1000);
            }
        }

    });

    $('#imgCropView').on('rcrop-changed', function () {
        if ($(".UploadCropImage").hasClass("btn-disable"))
            $(".UploadCropImage").removeClass("btn-disable");


        var srcOriginal = $(this).rcrop('getDataURL');
        var srcResized = $(this).rcrop('getDataURL', _cWidth, _cHeight);

        $(_cImgOneId).attr("src", "" + srcResized + "");
        debugger
        // This is case when when you want to show crop image on two images e.g. user profile page
        if ($(_cImgTwoId).length > 0)
            $(_cImgTwoId).attr("src", "" + srcResized + "");

        if ($(_cImgThreeId).length > 0)
            $(_cImgThreeId).attr("src", "" + srcResized + "");

        // if you want to strore base64 data into hidden field
        if ($(_cHFId).length > 0)
            $(_cHFId).val("" + srcResized + "");


    });
  

//CROP END==============================================================
    var _dynCount = -1;

    $("body").on("click", ".clearPic", function () {
        if ($(_cHFId).val() == ""){           
            $("#divImg_" + _dynCount).removeClass("show").addClass("hide");
            $("#cross_" + _dynCount).removeClass("show").addClass("hide");
        }
    });
    //DYNAMIC IMAGES
    $("body").on("click", ".cropBoxDynamic", function () {
        _cHFId = "";
        //_fileId = $(this).attr("data");
       
        var cTotalImg = parseInt($(this).attr("cTotalImg"))-1;
        for (var i = cTotalImg; i >=0 ; i--) {
            if ($.trim($("#hfPath_" + i).val()) == "" && _cHFId=="") {
                _cImgOneId = "#img_" + i;
                _cHFId = "#hfIB_" + i;
                $("#hfPath_" + i).val("Fill");
                $("#divImg_" + i).removeClass("hide").addClass("show");
                $("#cross_" + i).removeClass("hide").addClass("show");
                _dynCount = i;
               // return false;
            }
        }       
        
        _cWidth = parseInt($(this).attr("cWidth"));
        _cHeight = parseInt($(this).attr("cHeight"));

        // hide save button

        var attr = $(this).attr("cBtnSave");
        // For some browsers, `attr` is undefined; for others,
        // `attr` is false.  Check for both.
        //  debugger;
        if (typeof attr == "undefined" || attr == false) {
            $(".UploadCropImage").css({ display: "none" });
        }
       
        $("#imgCropView").attr("src", "");
        $("#imgCropView").rcrop('destroy');
        $("#squareCropModal").modal("show");


    });
});