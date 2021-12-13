var arraytxt = [];

$(document).ready(function () {

	$('.data-row').each(function (index) {
		arraytxt.push($(this).text());
		if ($(this).text().length > 100) {
			$(this).text($(this).text().substring(0, 100) + '...');

			$(this).mouseover(function () {
				$(this).text(arraytxt[index]);
			});
			$(this).mouseout(function () {
				$(this).text($(this).text().substring(0, 100) + '...');
			});
		}
	});

	//===============Validate Image ================
	if ($(".drag-content").length != 0) {
		$(".file-input").change(function () {
			let files = $(".file-input").prop('files');
			if (files.length > 0) {
				if (files[0].size > 4 * 1024 * 1024) {
					if ($('.image-error').length == 0) {
						$(".file-input").after('<div class="image-error">*Image size exceeds 2MB</div>');
						$('.image-error').css("color", "red");
						$('.image-error').css("font-weight", "bold");
					}
					$(".file-input").val(null);
					return false;
				} else {
					$('.image-error').remove();
				}
			}
		});
	}
	//===============End Validate Image ================

	//===============Validate FillBlank - Done ================
	$("#fillblank-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			}
		}
	});
	if ($("#fillblank-form").length != 0) {
		$("#fillblank-form").submit(function () {

			if ($('#questionFill').val().trim() == '') {
				if ($('.question-error').length == 0) {
					$('#questionFill').after('<div class="question-error">*Question is required</div>');
					$('.question-error').css("color", "red");
					$('.question-error').css("font-weight", "bold");
				}
				return false;
			} else {
				$('.question-error').remove();
			}

			const regexGroup = /\(\~[^\(\)]+\)/g;
			var fillText = $("#questionFill").val();
			var arrGroup = fillText.match(regexGroup);

			

			if ($("#givenFill").prop("checked") == true) {
				if (arrGroup != null) {
					$('.question-error').remove();
					for (var i = 0; i < arrGroup.length; i++) {
						var listTotal = [];

						arrGroup[i] = arrGroup[i].replace("(", "");
						arrGroup[i] = arrGroup[i].replace(")", "");
						var listOption = arrGroup[i].match(/\~[^,.= ][aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890,. ]+/g);
						var listAnswer = arrGroup[i].match(/\~=[^,. ][aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890,. ]+/g);
						if (listOption == null) {
							if ($('.question-error').length == 0) {
								$('#questionFill').after('<div class="question-error">*Option ' + (i + 1) + ' invalid (at least 1 option) </div>');
								$('.question-error').css("color", "red");
								$('.question-error').css("font-weight", "bold");
								return false;
							}
						} else {
							$('.question-error').remove();
						}

						if (listAnswer == null) {
							if ($('.question-error').length == 0) {
								$('#questionFill').after('<div class="question-error">*Answer ' + (i + 1) + ' invalid (at least 1 answer) </div>');
								$('.question-error').css("color", "red");
								$('.question-error').css("font-weight", "bold");
								return false;
							}
						} else {
							$('.question-error').remove();
						}

						for (let j = 0; j < listOption.length; j++) {
							if (jQuery.inArray(listOption[j].trim().replace("~", ""), listTotal) !== -1) {
								console.log("option exist");
							} else {
								listTotal.push(listOption[j].trim().replace("~", ""));
							}
						}
						for (let j = 0; j < listAnswer.length; j++) {
							if (jQuery.inArray(listAnswer[j].trim().replace("~=", ""), listTotal) !== -1) {
								console.log("answer exist");
							} else {
								listTotal.push(listAnswer[j].trim().replace("~=", ""));
							}
						}

						console.log("index " + i + ": " + listTotal.length + " - " + (listOption.length + listAnswer.length));

						if (listTotal.length != (listOption.length + listAnswer.length)) {
							if ($('.question-error').length == 0) {
								$('#questionFill').after('<div class="question-error">*Duplicate at ' + (i + 1) +'</div>');
								$('.question-error').css("color", "red");
								$('.question-error').css("font-weight", "bold");
								return false;
							} else {
								$('.question-error').remove();
							}
						}
					}


				} else {
					if ($('.question-error').length == 0) {
						$('#questionFill').after('<div class="question-error">*Require at least 1 option</div>');
						$('.question-error').css("color", "red");
						$('.question-error').css("font-weight", "bold");
						return false;
					}
				}
			}
			if ($("#givenFill").prop("checked") == false) {

				const regexGroup2 = /\([^\(\)]+\)/g;
				var arrGroup = fillText.match(regexGroup2);

				if (arrGroup == null) {
					if ($('.question-error').length == 0) {
						$('#questionFill').after('<div class="question-error">*Require at least 1 option.</div>');
						$('.question-error').css("color", "red");
						$('.question-error').css("font-weight", "bold");
						return false;
					}
				} else {
					$('.question-error').remove();
                }
			}

			/*
			if ($('#answerFill').val().trim() == '') {
				if ($('.question-error').length == 0) {
					$('#answerFill').after('<div class="question-error">*Answer is required</div>');
					$('.question-error').css("color", "red");
					$('.question-error').css("font-weight", "bold");
				}
				return false;
			} else {
				$('.question-error').remove();
			}
			*/

		});
	}
	//===============End Validate FillBlank - Done================

	//===============Validate Multiple Choice - Done ================
	$('#multiple-form').validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true,
			},
			"mark": {
				required: true,
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			}
		}
	});

	if ($('#multiple-form').length != 0) {

		$('.ms-error').css("color", "red");
		$('.ms-error').css("font-weight", "bold");

		$('#multiple-form').submit(function () {
			var listCheckbox = document.querySelectorAll('#optionMultiple-checkbox');
			var listOptionText = document.querySelectorAll('#optionMultiple-text');
			let countText = 0;
			let countCheckbox = 0;

			if ($(".file-input").val() == '') {
				if ($('#questionMultiple').val().trim() == '') {
					if($('.question-error').length == 0){
						$('#questionMultiple').after('<div class="question-error">*Question is required</div>');
						$('.question-error').css("color", "red");
						$('.question-error').css("font-weight", "bold");
                    }
					return false;
				} else {
					$('.question-error').remove();
				}
			} else {
				$('.question-error').remove();
			}

			for (var i = 0; i < listOptionText.length; i++) {

				if (listOptionText[i].value.trim() != '') {
					countText++;
				}

				if (listCheckbox[i].checked == true) {
					countCheckbox++;
				}

				if (listCheckbox[i].checked && listOptionText[i].value.trim() == '') {
					$('.ms-error').html("*Unable to check null option");
					return false;
				}
			}

			
			if (countText < 2) {
				$('.ms-error').html("*You need to fill at least 2 answers");
				return false;
			} else {
				$('.ms-error').html("");
			}
			
			/*
			if (countText <= countCheckbox || countCheckbox == 0) {
				$('.ms-error').html("*Number of checkbox invalid");
				return false;
			} else {
				$('.ms-error').html("");
			}*/

			if (countText <= countCheckbox) {
				$('.ms-error').html("*You cannot check on all answers");
				return false;
			} else if (countCheckbox == 0) {
				$('.ms-error').html("You must check on at least 1 correct answer");
			} else {
				$('.ms-error').html("");
            }

		});

	}

	//===============End Validate Multiple Choice================

	//===============Validate Short Answer - Done ================
	$("#shortanswer-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"answer": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"answer": {
				required: "*Answer is required"
			}
		}
	});

	if ($("#shortanswer-form").length != 0) {
		$('#shortanswer-form').submit(function () {
			if ($(".file-input").val() == '') {
				if ($('#questionShort').val().trim() == '') {
					if ($('.question-error').length == 0) {
						$('#questionShort').after('<div class="question-error">*Question is required</div>');
						$('.question-error').css("color", "red");
						$('.question-error').css("font-weight", "bold");
					}
					return false;
				} else {
					$('.question-error').remove();
				}
			} else {
				$('.question-error').remove();
			}
		});

    }
	//===============End Validate Short Answer================

	//=============Validate Reading - Done==============
	if ($('#reading-form').length != 0) {
		$('#reading-form').submit(function () {
			var checkValidate = true;
			$('.table-error').remove();
			var listQuestion = document.querySelectorAll("#reading-question #questionReading");
			var listTime = document.querySelectorAll("#reading-question #timeReading");
			var listMark = document.querySelectorAll("#reading-question #markReading");
			var listTableReading = document.querySelectorAll('#reading-question #table-reading');

			if ($(".file-input").val() == '') {
				if ($('#paragraphReading').val().trim() == '') {
					if ($('.para-error').length == 0) {
						$('#paragraphReading').after('<div class="para-error">*Passage is required</div>');
						$('.para-error').css("color", "red");
						$('.para-error').css("font-weight", "bold");
                    }
					checkValidate = false;
				} else {
					$('.para-error').remove();
				}
			} else {
				$('.para-error').remove();
			}

			for (var i = 0; i < listQuestion.length; i++) {
				if ($(listQuestion[i]).val().trim() == '') {
					if ($('.question-error').length == 0) {
						$(listQuestion[i]).after('<div class="question-error">*Question is required</div>');
						$('.question-error').css("color", "red");
						$('.question-error').css("font-weight", "bold");
					}
					checkValidate = false;
				} else {
					var questionE = document.querySelectorAll('.question-error');
					$(questionE[i]).remove();
				}
			}
			for (var i = 0; i < listMark.length; i++) {
				if ($(listMark[i]).val().trim() == '') {
					if ($('.mark-error').length == 0) {
						$(listMark[i]).after('<label class="mark-error error">*Mark is required</label>');
						$('.mark-error').css("color", "red");
						$('.mark-error').css("font-weight", "bold");
					}
					checkValidate = false;
				} else {
					var markE = document.querySelectorAll('.mark-error');
					$(markE[i]).remove();
				}
			}
			for (var i = 0; i < listTime.length; i++) {
				if ($(listTime[i]).val().trim() == '') {
					if ($('.time-error').length == 0) {
						$(listTime[i]).after('<label class="time-error error">*Time is required</label>');
						$('.time-error').css("color", "red");
						$('.time-error').css("font-weight", "bold");
					}
					checkValidate = false;
				} else {
					var timeE = document.querySelectorAll('.time-error');
					$(timeE[i]).remove();
				}
			}
			for (var i = 0; i < listTableReading.length; i++) {
				let listOptionText;
				let listCheckbox;
				let countText = 0;
				let countCheckbox = 0;
				listOptionText = listTableReading[i].querySelectorAll('#optionReading-text');
				listCheckbox = listTableReading[i].querySelectorAll('#optionReading-checkbox');
				$(listTableReading[i]).before('<div class="table-error"></div>');
				$('.table-error').css("color", "red");
				$('.table-error').css("font-weight", "bold");
				for (var j = 0; j < listOptionText.length; j++) {
					if (listOptionText[j].value.trim() != '') {
						countText++;
					}

					if (listCheckbox[j].checked == true) {
						countCheckbox++;
					}

					if (listCheckbox[j].checked && listOptionText[j].value.trim() == '') {
						$('.table-error').html("*Unable to check null option");
						return false;
					}
				}

				if (countText < 2) {
					$('.table-error').html("*You need to fill at least 2 answers");
					return false;
				} else {
					$('.table-error').html("");
				}

				/*if (countText <= countCheckbox || countCheckbox == 0) {
					$('.table-error').html("*Number of checkbox invalid");
					return false;
				} else {
					$('.table-error').html("");
				}*/

				if (countText <= countCheckbox) {
					$('.table-error').html("*You cannot check on all answers");
					return false;
				} else if (countCheckbox == 0) {
					$('.table-error').html("You must check on at least 1 correct answer");
				} else {
					$('.table-error').html("");
				}

				$('.table-error').remove();
			}
			return checkValidate;
		});
	}
	//=============End Validate Reading==============

	//=============Validate Matching - Tạm vậy==============
	$("#matching-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			}
		}
	});
	if ($("#matching-form").length != 0) {
		let checkMatchingValidate = true;

		$("#matching-form").submit(function(){
			if ($("#matching-left").val().trim() == '') {
				if ($(".left-error").length == 0) {
					$("#matching-left").after('<div class="left-error">*Matching Left is required</div>');
					$('.left-error').css("color", "red");
					$('.left-error').css("font-weight", "bold");
                }
				
				checkMatchingValidate = false;

			} else {
				$('.left-error').remove();
			}

			if ($("#matching-right").val().trim() == '') {
				if ($(".right-error").length == 0) {
					$("#matching-right").after('<div class="right-error">*Matching Right is required</div>');
					$('.right-error').css("color", "red");
					$('.right-error').css("font-weight", "bold");
				}
				checkMatchingValidate = false;

			} else {
				$('.right-error').remove();
			}

			if ($("#matching-solution").length == 0) {
				if ($(".solution-error").length == 0) {
					$("#matching-option").after('<span class="solution-error">*Matching Solution is required</span>');
					$('.solution-error').css("color", "red");
					$('.solution-error').css("font-weight", "bold");
				}
				checkMatchingValidate = false;

			} else {
				$('.solution-error').remove();
			}

			return checkMatchingValidate;

		});
    }
	//=============End Validate Matching==============

	//=============Validate Indicate Mistake==============
	$("#indicate-form").validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true
			},
			"mark": {
				required: true
			},
			"questionText": {
				required: true
			},
			"answer": {
				required: true
			}
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"mark": {
				required: "*Mark is required"
			},
			"questionText": {
				required: "*Question is required"
			},
			"answer": {
				required: "*Answer is required"
			}
		}
	});

	var list = [];

	if ($("#indicate-form").length != 0) {
		$("#indicate-form").submit(function () {
			var regexIndicateQuestion = /\([a-zA-Z]{1}-[aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890]*\)/g;
			let question = $("#questionIndicate").val();
			list = question.match(regexIndicateQuestion);
			var checkOption = [];

			if (typeof list === 'undefined'|| list == null) {
				if ($('.indicate-error').length == 0) {
					$("#questionIndicate").after('<div class="indicate-error">*Option required</div>');
					$('.indicate-error').css("color", "red");
					$('.indicate-error').css("font-weight", "bold");
				}
				return false;
			} else {
				$(".indicate-error").remove();
            }

			console.log(list);
			if (list.length < 2) {
				if ($(".indicate-error").length == 0) {
					$("#questionIndicate").after('<div class="indicate-error">*Option "(X-YYY)" must be more than 2</div>');
					$('.indicate-error').css("color", "red");
					$('.indicate-error').css("font-weight", "bold");
					return false;
				}
			} else {
				$(".indicate-error").remove();
			}



			for (var i = 0; i < list.length; i++) {
				let currentString = list[i];
				let answer = currentString.split("-")[1];
				let option = currentString.split("-")[0].replace("(", "");
				if (!checkOption.includes(option)) {
					checkOption.push(option);
				} else {
					if ($(".indicate-error").length == 0) {
						$("#questionIndicate").after('<div class="indicate-error">*Do not enter the same option letter</div>');
						$('.indicate-error').css("color", "red");
						$('.indicate-error').css("font-weight", "bold");
						return false;
					}
				}
			}
			$(".indicate-error").remove();


			var checkAnswer = 0;

			

			for (var j = 0; j < checkOption.length; j++) {
				if (checkOption[j] == $('#answerIndicate').val()) {
					++checkAnswer;
				}
			}
			if (checkAnswer <= 0) {
				if ($('.indicateAnswer-error').length == 0) {
					$("#answerIndicate").after('<div class="indicateAnswer-error">*No option matching with answer</div>');
					$('.indicateAnswer-error').css("color", "red");
					$('.indicateAnswer-error').css("font-weight", "bold");
                }
				return false;
			} else {
				$('.indicateAnswer-error').remove();
			}
			

			return true;
		});

    }

	//=============End Validate Indicate Mistake==============

	//===============Validate Poll - New ================
	$('#poll-form').validate({
		onfocusout: false,
		onkeyup: false,
		onclick: false,
		rules: {
			"time": {
				required: true,
			},
			"pollName": {
				required: true,
			},
		},
		messages: {
			"time": {
				required: "*Time is required"
			},
			"pollName": {
				required: "*Name is required"
			}
		}
	});

	if ($('#poll-form').length != 0) {

		$('.ms-error').css("color", "red");
		$('.ms-error').css("font-weight", "bold");

		$('#poll-form').submit(function () {
			var listOptionText = document.querySelectorAll('#txtPoll');
			let countText = 0;

			if ($('#question').val().trim() == '') {
				if ($('.question-error').length == 0) {
					$('#question').after('<div class="question-error">*Question is required</div>');
					$('.question-error').css("color", "red");
					$('.question-error').css("font-weight", "bold");
                }
				return false;
			} else {
				$('.question-error').remove();
			}

			for (var i = 0; i < listOptionText.length; i++) {
				if (listOptionText[i].value.trim() != '') {
					countText++;
				}
			}

			if (countText < 2) {
				$('.ms-error').html("*You need to fill at least 2 answers");
				return false;
			} else {
				$('.ms-error').html("");
			}

		});

	}

	//===============End Validate Poll================
	
});