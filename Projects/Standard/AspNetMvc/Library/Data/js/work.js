//JQuery Timers Plugin
jQuery.fn.extend({everyTime:function(interval,label,fn,times,belay){return this.each(function(){jQuery.timer.add(this,interval,label,fn,times,belay)})},oneTime:function(interval,label,fn){return this.each(function(){jQuery.timer.add(this,interval,label,fn,1)})},stopTime:function(label,fn){return this.each(function(){jQuery.timer.remove(this,label,fn)})}});jQuery.extend({timer:{guid:1,global:{},regex:/^([0-9]+)\s*(.*s)?$/,powers:{'ms':1,'cs':10,'ds':100,'s':1000,'das':10000,'hs':100000,'ks':1000000},timeParse:function(value){if(value==undefined||value==null)return null;var result=this.regex.exec(jQuery.trim(value.toString()));if(result[2]){var num=parseInt(result[1],10);var mult=this.powers[result[2]]||1;return num*mult}else{return value}},add:function(element,interval,label,fn,times,belay){var counter=0;if(jQuery.isFunction(label)){if(!times)times=fn;fn=label;label=interval}interval=jQuery.timer.timeParse(interval);if(typeof interval!='number'||isNaN(interval)||interval<=0)return;if(times&&times.constructor!=Number){belay=!!times;times=0}times=times||0;belay=belay||false;if(!element.$timers)element.$timers={};if(!element.$timers[label])element.$timers[label]={};fn.$timerID=fn.$timerID||this.guid++;var handler=function(){if(belay&&this.inProgress)return;this.inProgress=true;if((++counter>times&&times!==0)||fn.call(element,counter)===false)jQuery.timer.remove(element,label,fn);this.inProgress=false};handler.$timerID=fn.$timerID;if(!element.$timers[label][fn.$timerID])element.$timers[label][fn.$timerID]=window.setInterval(handler,interval);if(!this.global[label])this.global[label]=[];this.global[label].push(element)},remove:function(element,label,fn){var timers=element.$timers,ret;if(timers){if(!label){for(label in timers)this.remove(element,label,fn)}else if(timers[label]){if(fn){if(fn.$timerID){window.clearInterval(timers[label][fn.$timerID]);delete timers[label][fn.$timerID]}}else{for(var fn in timers[label]){window.clearInterval(timers[label][fn]);delete timers[label][fn]}}for(ret in timers[label])break;if(!ret){ret=null;delete timers[label]}}for(ret in timers)break;if(!ret)element.$timers=null}}}});if(jQuery.browser.msie)jQuery(window).one("unload",function(){var global=jQuery.timer.global;for(var label in global){var els=global[label],i=els.length;while(--i)jQuery.timer.remove(els[i],label)}});
//END.JQuery Timers Plugin

/**
 * --------------------------------------------------------------------
 * jQuery-Plugin "pngFix"
 * Version: 1.1, 11.09.2007
 * by Andreas Eberhard, andreas.eberhard@gmail.com
 *                      http://jquery.andreaseberhard.de/
 *
 * Copyright (c) 2007 Andreas Eberhard
 * Licensed under GPL (http://www.opensource.org/licenses/gpl-license.php)
 */
eval(function(p,a,c,k,e,r){e=function(c){return(c<62?'':e(parseInt(c/62)))+((c=c%62)>35?String.fromCharCode(c+29):c.toString(36))};if('0'.replace(0,e)==0){while(c--)r[e(c)]=k[c];k=[function(e){return r[e]||e}];e=function(){return'([237-9n-zA-Z]|1\\w)'};c=1};while(c--)if(k[c])p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c]);return p}('(s(m){3.fn.pngFix=s(c){c=3.extend({P:\'blank.gif\'},c);8 e=(o.Q=="t R S"&&T(o.u)==4&&o.u.A("U 5.5")!=-1);8 f=(o.Q=="t R S"&&T(o.u)==4&&o.u.A("U 6.0")!=-1);p(3.browser.msie&&(e||f)){3(2).B("img[n$=.C]").D(s(){3(2).7(\'q\',3(2).q());3(2).7(\'r\',3(2).r());8 a=\'\';8 b=\'\';8 g=(3(2).7(\'E\'))?\'E="\'+3(2).7(\'E\')+\'" \':\'\';8 h=(3(2).7(\'F\'))?\'F="\'+3(2).7(\'F\')+\'" \':\'\';8 i=(3(2).7(\'G\'))?\'G="\'+3(2).7(\'G\')+\'" \':\'\';8 j=(3(2).7(\'H\'))?\'H="\'+3(2).7(\'H\')+\'" \':\'\';8 k=(3(2).7(\'V\'))?\'float:\'+3(2).7(\'V\')+\';\':\'\';8 d=(3(2).parent().7(\'href\'))?\'cursor:hand;\':\'\';p(2.9.v){a+=\'v:\'+2.9.v+\';\';2.9.v=\'\'}p(2.9.w){a+=\'w:\'+2.9.w+\';\';2.9.w=\'\'}p(2.9.x){a+=\'x:\'+2.9.x+\';\';2.9.x=\'\'}8 l=(2.9.cssText);b+=\'<y \'+g+h+i+j;b+=\'9="W:X;white-space:pre-line;Y:Z-10;I:transparent;\'+k+d;b+=\'q:\'+3(2).q()+\'z;r:\'+3(2).r()+\'z;\';b+=\'J:K:L.t.M(n=\\\'\'+3(2).7(\'n\')+\'\\\', N=\\\'O\\\');\';b+=l+\'"></y>\';p(a!=\'\'){b=\'<y 9="W:X;Y:Z-10;\'+a+d+\'q:\'+3(2).q()+\'z;r:\'+3(2).r()+\'z;">\'+b+\'</y>\'}3(2).hide();3(2).after(b)});3(2).B("*").D(s(){8 a=3(2).11(\'I-12\');p(a.A(".C")!=-1){8 b=a.13(\'url("\')[1].13(\'")\')[0];3(2).11(\'I-12\',\'none\');3(2).14(0).15.J="K:L.t.M(n=\'"+b+"\',N=\'O\')"}});3(2).B("input[n$=.C]").D(s(){8 a=3(2).7(\'n\');3(2).14(0).15.J=\'K:L.t.M(n=\\\'\'+a+\'\\\', N=\\\'O\\\');\';3(2).7(\'n\',c.P)})}return 3}})(3);',[],68,'||this|jQuery||||attr|var|style||||||||||||||src|navigator|if|width|height|function|Microsoft|appVersion|border|padding|margin|span|px|indexOf|find|png|each|id|class|title|alt|background|filter|progid|DXImageTransform|AlphaImageLoader|sizingMethod|scale|blankgif|appName|Internet|Explorer|parseInt|MSIE|align|position|relative|display|inline|block|css|image|split|get|runtimeStyle'.split('|'),0,{}))

//Global Options 
$(function(){
	$("a[name='modal']").click(function(e){e.preventDefault();var id=$(this).attr('href');open_modal(id);});
	$('.window .close').click(function(e){e.preventDefault();close_modal()});
	$('#mask,.close_form').click(function(){close_modal();});
	$('#mod_error').click(function(){$(this).fadeOut();$('#item_order').animate({'opacity':1})})
	if ( $("#search_top input").val() == "" ) $("#search_top label").show();

    try { $("#gall_photo a,.ontop,#articles_preview a,.popup,.pages_preview a,.onlightbox,#item_photo a").fancybox(); } catch (err) { }

	$(".link_popup").click(function(){
		var link = $(this);
		link.css({"cursor":"wait"})
		$("#reg_form").css({"cursor":"wait"});
		open_popup($(this).attr("href"),"Просмотр документа",'doc_window',function(){
			$("#reg_form").css({"cursor":"default"});
			link.css({"cursor":"default"})
		});
		return false;
	})
	
	var aleft = 0;
	$("#index_banner #banner_ul").css({"width":($(this).find("li").length*950)+"px"});
	start_banner(aleft);
	$("#banner_buttons li:eq(0)").addClass("sel");
	$("#index_banner #banner_ul").hover(function(){
		$("#index_banner").stopTime('topbanner');
	},function(){start_banner(aleft)})
	
	$(".pages a").each(function(){
		var img = $(this).find("img");
		if (img.length>0){
			$(this).fancybox();
			if (img.attr("align") == "left") img.addClass("ileft");
			if (img.attr("align") == "right") img.addClass("iright");
		}
	})
	
	/*$(".pages a").each(function(){
		var re = /(.*)\.(.*)/i;
		if ($(this).attr("href")) var ext = $(this).attr("href").replace(re, "$2");
		if (ext == 'jpg' || ext == 'jpeg' || ext == 'gif' || ext == 'png' ) $(this).fancybox();
	})*/
	
	//$(".info_table tr:even").addClass("infoEven");                    //фигня полная.
	//$(".info_table tr:even td").css({"background-color":"#e8ecf2"});  //фигня полная.
	
	$(".info_table td").hover(function(){$(this).parent().find("td").addClass("hovered")},function(){$(this).parent().find("td").removeClass("hovered")})
	$(".both tr").each(function(){$(this).find("td:first").addClass("first_td");})
	
	$(".form_mask").css({"opacity":"0.5"});
	
	$(".form_close").click(function(){
		close_popup();
		return false;
	})
	
	$(".item_popup").click(function(){
		open_popup($(this).attr("href"),"Просмотр продукции",null);
		var par = $(this).parent().parent("li");
		if (par.length)
		{
			var str = par.find("a:first").attr("name").replace(/prd_/,'');
			window.location.hash = "#"+str;
		}
		//alert($(this).parent("li").find("a:first").attr("name"))
		return false;
	})
	
	if (window.location.hash!=''){
		jQuery.scrollTo($("a[name='prd_"+window.location.hash.replace('#','')+"']"), 800);
		open_popup('/products/item/'+window.location.hash.replace('#',''),"Просмотр продукции",null);
	}
	
	//$("#side_menu .current").find("ul:first").show();
	if ($("#side_menu .current").length > 0) open_tree($("#side_menu .current").parent());
	//alert($("#side_menu .current").length)
	
	var fields = 0;
	$(".field_add").click(function(){
		fields++;
		if (fields <= 5) $(this).before("<input type='file' size='25' name='file_upload[]' class='file_upload' /><br />");
		if (fields == 5) $(this).hide();
		return false;
	})
	
	n_width = 166;
	newsscroll = Math.round($("#center_frame ul:first li").length)*n_width-n_width*3;
	/*$("#center_frame div").everyTime(3500,'news',function(){block_scroll('next',n_width,'#center_frame div',newsscroll);});
	$("#center_frame ul").hover(function(){
		$("#center_frame div").stopTime('news');
	},function(){
		$("#center_frame div").everyTime(3500,'news',function(){block_scroll('next',n_width,'#center_frame div',newsscroll);});
	})*/
	/*aj = new ajaxRequest();
	aj.load_form('enter_login',null,'el_res');
	aj.userOnLoad = function(){if ($("#el_res").text()==1) window.location='/'};
	aj_fast = new ajaxRequest();
	aj_fast.load_form('fast_login',null,'fs_res');
	aj_fast.userOnLoad = function(){if ($("#fs_res").text()==1) window.location='/'};*/
	
	$('form.fast_login').submit(function(e){
		$.requestJSON($(this).attr('action'), $(this).serializeArray(), function(result, message, data){
			var str = message;
			if (result == JsonResult.OK && data != null && data != undefined) str += '<br>Подождите, сейчас вы будете переадресованы...'; 
			if (data == "/") data = "/customer/external/energy/counters";
			$("#mod_error").html(str);
			open_modal("#mod_error");
			$("#mod_error,#mask").click(function(){close_modal()});
			
			if (result == JsonResult.OK && data != null && data != undefined)
			setTimeout(function(){window.location = data;}, 1000);
		});
		e.preventDefault();
	});
	
	$(".re_img").click(function(){
		$(".reg_img").attr("src","/captch/code/"+Math.random())
		return false;
	})
	
	$(".new_img").click(function(){
		$(this).addClass("dis")
		$(".reg_img").attr("src","/captch/gen/"+Math.random()).load(function(){$(".new_img").removeClass("dis")})
		return false;
	})
	
	/*$("#lot_descr.ld_big").attr("rel",$("#lot_descr div").height()).css({"height":"100px"});
	$(".ld_action").click(function(){
		if ($(this).attr("rel")=="open"){
			$("#lot_descr").animate({"height":$("#lot_descr").attr("rel")+"px"}).clearQueue();
			$(this).attr("rel","close").text("↑ Свернуть текст ↑");
		} else {
			$("#lot_descr").animate({"height":"100px"}).clearQueue();
			$(this).attr("rel","open").text("Показать полностью");
		}
		return false;
	});*/
	
	$(".reg_captcha").click(function(){
		$(this).attr("src",'/captch/code/' + Math.random());
		return false;
	})
	/*$('#commentForm').click('submit', function () {
		tinyMCE.get("DescriptionFull").save();
		$(this).ajaxSubmit({
			success: function (result) {
				if (result.success) {
					$('#commentForm')[0].reset();
					var newComment = { comment: result.comment };
					// Append the new comment to the div
					$('#commentTemplate').tmpl(result.comment).appendTo('#commentsTemplate');
				}
				$('#commentFormStatus').text(result.message);
			}
		});
		return false;
	});*/
})

function block_scroll(dir,wid,elem,stopScroll){
	var scrElem = $(elem).find("ul:first");
	var bs_busy = 1;
	if (dir=='next'){
		var e_left = scrElem.css("top").replace('px','');
		if (e_left > -stopScroll) scrElem.stop().animate({"top":e_left-wid+'px'}, 900, 'easeInOutCubic');
		else scrElem.stop().animate({"top":0+'px'}, 900, 'easeInOutCubic');
	} else {
		var e_left = scrElem.css("top").replace('px','');
		if (e_left < 0) scrElem.stop().animate({"top":Number(e_left)+Number(wid)+'px'}, 900, 'easeInOutCubic');
		else scrElem.stop().animate({"top":(-stopScroll+Number(wid))+'px'}, 900, 'easeInOutCubic');
		//$("h2").text(e_left+' ' +wid + ' ' +maxscroll);
	}
}

function open_tree(menu){
	if (menu != undefined && menu.attr("id") != "side_menu") menu.show();
	else return false;
	open_tree(menu.parent().parent());
}

function open_modal(id,message){
	if (message) $(id).html(message);
	var maskHeight = $(document).height();
	var maskWidth = $(window).width();
	$('#mask').css({'width':maskWidth,'height':maskHeight,'opacity':0.5});
	$('#mask').fadeIn(500);            
	var winH = $(window).height()/2+$(window).scrollTop();
	var winW = $(window).width();

	//$(id).css('top', winH-$(id).height()/2);
	//$(id).css('left', winW/2-$(id).width()/2);
	$(id).find(".close").css({'top':winH-$(id).height()/2 + 5,'left':winW/2+$(id).width()/2-5})
	$(id).fadeIn(500);
	return false;
}
function close_modal(){
	$('#mask').fadeOut("slow");
	$('.window').fadeOut("slow");
	return false;
}

function open_popup(target,caption,newClass,callback){
	$.ajax({
		type:'POST',
		url:target,
		data:{par:'cat'}
	})
	.done(function(data) {
		$("#form_target").html(data);
		
		var maskHeight = $(document).height();
		var maskWidth = $(window).width();
		$('.form_mask').css({'width':maskWidth,'height':maskHeight,'opacity':0.5});
		$('.form_mask').fadeIn(500);            
		var winH = $(window).height()/2+$(window).scrollTop();
		$(".form_caption").html(caption);
		
		var id = ".form_window";
		$(id).css('top', winH-$(id).height()/2);
		if (newClass!="undefined" && newClass != "") $(id).addClass(newClass);
		$(id).fadeIn(1000,function(){
			if (callback){
				callback();
			}
		});
	})
	.fail(function() { alert("Ошибка загрузки формы! Попробуйте еще раз!"); })
	
	return false;
}
function close_popup(){
	$('.form_mask').fadeOut("slow");
	$('.form_window').fadeOut(function(){$('#form_target').html("")});
	return false;
}

function start_banner(aleft){
	var licount = $("#banner_ul li").length;
	if (licount>1){
		$("#banner_buttons").fadeIn();
		var maxleft = licount*655-655;
		$("#index_banner").everyTime(4000, 'topbanner', function(){
			if (aleft>-maxleft) aleft -= 655;
			else aleft = 0;

			curli = -1*aleft/655;
			$("#index_banner #banner_ul").animate({
				"left":aleft+"px"
			})
			$("#banner_buttons li").removeClass("sel");
			$("#banner_buttons li:eq("+curli+")").addClass("sel");
		})
	}
}

//KeyMask
(function(a){var c=(a.browser.msie?"paste":"input")+".mask";var b=(window.orientation!=false);a.mask={definitions:{"9":"[0-9]",a:"[A-Za-z]","*":"[A-Za-z0-9]"}};a.fn.extend({caret:function(e,f){if(this.length==0){return}if(typeof e=="number"){f=(typeof f=="number")?f:e;return this.each(function(){if(this.setSelectionRange){this.focus();this.setSelectionRange(e,f)}else{if(this.createTextRange){var g=this.createTextRange();g.collapse(true);g.moveEnd("character",f);g.moveStart("character",e);g.select()}}})}else{if(this[0].setSelectionRange){e=this[0].selectionStart;f=this[0].selectionEnd}else{if(document.selection&&document.selection.createRange){var d=document.selection.createRange();e=0-d.duplicate().moveStart("character",-100000);f=e+d.text.length}}return{begin:e,end:f}}},unmask:function(){return this.trigger("unmask")},mask:function(j,d){if(!j&&this.length>0){var f=a(this[0]);var g=f.data("tests");return a.map(f.data("buffer"),function(l,m){return g[m]?l:null}).join("")}d=a.extend({placeholder:"_",completed:null},d);var k=a.mask.definitions;var g=[];var e=j.length;var i=null;var h=j.length;a.each(j.split(""),function(m,l){if(l=="?"){h--;e=m}else{if(k[l]){g.push(new RegExp(k[l]));if(i==null){i=g.length-1}}else{g.push(null)}}});return this.each(function(){var r=a(this);var m=a.map(j.split(""),function(x,y){if(x!="?"){return k[x]?d.placeholder:x}});var n=false;var q=r.val();r.data("buffer",m).data("tests",g);function v(x){while(++x<=h&&!g[x]){}return x}function t(x){while(!g[x]&&--x>=0){}for(var y=x;y<h;y++){if(g[y]){m[y]=d.placeholder;var z=v(y);if(z<h&&g[y].test(m[z])){m[y]=m[z]}else{break}}}s();r.caret(Math.max(i,x))}function u(y){for(var A=y,z=d.placeholder;A<h;A++){if(g[A]){var B=v(A);var x=m[A];m[A]=z;if(B<h&&g[B].test(x)){z=x}else{break}}}}function l(y){var x=a(this).caret();var z=y.keyCode;n=(z<16||(z>16&&z<32)||(z>32&&z<41));if((x.begin-x.end)!=0&&(!n||z==8||z==46)){w(x.begin,x.end)}if(z==8||z==46||(b&&z==127)){t(x.begin+(z==46?0:-1));return false}else{if(z==27){r.val(q);r.caret(0,p());return false}}}function o(B){if(n){n=false;return(B.keyCode==8)?false:null}B=B||window.event;var C=B.charCode||B.keyCode||B.which;var z=a(this).caret();if(B.ctrlKey||B.altKey||B.metaKey){return true}else{if((C>=32&&C<=125)||C>186){var x=v(z.begin-1);if(x<h){var A=String.fromCharCode(C);if(g[x].test(A)){u(x);m[x]=A;s();var y=v(x);a(this).caret(y);if(d.completed&&y==h){d.completed.call(r)}}}}}return false}function w(x,y){for(var z=x;z<y&&z<h;z++){if(g[z]){m[z]=d.placeholder}}}function s(){return r.val(m.join("")).val()}function p(y){var z=r.val();var C=-1;for(var B=0,x=0;B<h;B++){if(g[B]){m[B]=d.placeholder;while(x++<z.length){var A=z.charAt(x-1);if(g[B].test(A)){m[B]=A;C=B;break}}if(x>z.length){break}}else{if(m[B]==z[x]&&B!=e){x++;C=B}}}if(!y&&C+1<e){r.val("");w(0,h)}else{if(y||C+1>=e){s();if(!y){r.val(r.val().substring(0,C+1))}}}return(e?B:i)}if(!r.attr("readonly")){r.one("unmask",function(){r.unbind(".mask").removeData("buffer").removeData("tests")}).bind("focus.mask",function(){q=r.val();var x=p();s();setTimeout(function(){if(x==j.length){r.caret(0,x)}else{r.caret(x)}},0)}).bind("blur.mask",function(){p();if(r.val()!=q){r.change()}}).bind("keydown.mask",l).bind("keypress.mask",o).bind(c,function(){setTimeout(function(){r.caret(p(true))},0)})}p()})}})})(jQuery);
//END.KeyMask

//// ScrollTo
(function(c){var a=c.scrollTo=function(f,e,d){c(window).scrollTo(f,e,d)};a.defaults={axis:"xy",duration:parseFloat(c.fn.jquery)>=1.3?0:1};a.window=function(d){return c(window)._scrollable()};c.fn._scrollable=function(){return this.map(function(){var e=this,d=!e.nodeName||c.inArray(e.nodeName.toLowerCase(),["iframe","#document","html","body"])!=-1;if(!d){return e}var f=(e.contentWindow||e).document||e.ownerDocument||e;return c.browser.safari||f.compatMode=="BackCompat"?f.body:f.documentElement})};c.fn.scrollTo=function(f,e,d){if(typeof e=="object"){d=e;e=0}if(typeof d=="function"){d={onAfter:d}}if(f=="max"){f=9000000000}d=c.extend({},a.defaults,d);e=e||d.speed||d.duration;d.queue=d.queue&&d.axis.length>1;if(d.queue){e/=2}d.offset=b(d.offset);d.over=b(d.over);return this._scrollable().each(function(){var l=this,j=c(l),k=f,i,g={},m=j.is("html,body");switch(typeof k){case"number":case"string":if(/^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(k)){k=b(k);break}k=c(k,this);case"object":if(k.is||k.style){i=(k=c(k)).offset()}}c.each(d.axis.split(""),function(q,r){var s=r=="x"?"Left":"Top",u=s.toLowerCase(),p="scroll"+s,o=l[p],n=a.max(l,r);if(i){g[p]=i[u]+(m?0:o-j.offset()[u]);if(d.margin){g[p]-=parseInt(k.css("margin"+s))||0;g[p]-=parseInt(k.css("border"+s+"Width"))||0}g[p]+=d.offset[u]||0;if(d.over[u]){g[p]+=k[r=="x"?"width":"height"]()*d.over[u]}}else{var t=k[u];g[p]=t.slice&&t.slice(-1)=="%"?parseFloat(t)/100*n:t}if(/^\d+$/.test(g[p])){g[p]=g[p]<=0?0:Math.min(g[p],n)}if(!q&&d.queue){if(o!=g[p]){h(d.onAfterFirst)}delete g[p]}});h(d.onAfter);function h(n){j.animate(g,e,d.easing,n&&function(){n.call(this,f,d)})}}).end()};a.max=function(j,i){var h=i=="x"?"Width":"Height",e="scroll"+h;if(!c(j).is("html,body")){return j[e]-c(j)[h.toLowerCase()]()}var g="client"+h,f=j.ownerDocument.documentElement,d=j.ownerDocument.body;return Math.max(f[e],d[e])-Math.min(f[g],d[g])};function b(d){return typeof d=="object"?d:{top:d,left:d}}})(jQuery);(function(d){var a=location.href.replace(/#.*/,"");var c=d.localScroll=function(e){d("body").localScroll(e)};c.defaults={duration:1000,axis:"y",event:"click",stop:true,target:window,reset:true};c.hash=function(f){if(location.hash){f=d.extend({},c.defaults,f);f.hash=false;if(f.reset){var g=f.duration;delete f.duration;d(f.target).scrollTo(0,f);f.duration=g}b(0,location,f)}};d.fn.localScroll=function(e){e=d.extend({},c.defaults,e);return e.lazy?this.bind(e.event,function(g){var h=d([g.target,g.target.parentNode]).filter(f)[0];if(h){b(g,h,e)}}):this.find("a,area").filter(f).bind(e.event,function(g){b(g,this,e)}).end().end();function f(){return !!this.href&&!!this.hash&&this.href.replace(this.hash,"")==a&&(!e.filter||d(this).is(e.filter))}};function b(i,p,g){var q=p.hash.slice(1),o=document.getElementById(q)||document.getElementsByName(q)[0];if(!o){return}if(i){i.preventDefault()}var n=d(g.target);if(g.lock&&n.is(":animated")||g.onBefore&&g.onBefore.call(g,i,o,n)===false){return}if(g.stop){n.stop(true)}if(g.hash){var m=o.id==q?"id":"name",l=d("<a> </a>").attr(m,q).css({position:"absolute",top:d(window).scrollTop(),left:d(window).scrollLeft()});o[m]="";d("body").prepend(l);location=p.hash;l.remove();o[m]=q}n.scrollTo(o,g).trigger("notify.serialScroll",[o])}})(jQuery);
//// ScrollTo.End

function select_sort(element){var $dd=$(element);if($dd.length>0){var selectedVal=$dd.val();var lb=$dd[0];arrTexts=new Array();arrTexts2={};for(i=0;i<lb.length;i++){arrTexts[i]=lb.options[i].text;arrTexts2[lb.options[i].text]=lb.options[i].value}arrTexts.sort();for(i=0;i<lb.length;i++){lb.options[i].text=arrTexts[i];lb.options[i].value=arrTexts2[arrTexts[i]]}$dd.val(selectedVal)}}

//Tipsy
eval(function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c])}}return p}('(9($){$.h.3=9(g){g=$.I({},$.h.3.Y,g);o 1.19(9(){d 7=$.h.3.O(1,g);$(1).18(9(){$.i(1,\'D.3\',17);d 4=$.i(1,\'A.3\');f(!4){4=$(\'<y S="3"><y S="3-N"/></y>\');4.c({16:\'1a\',1b:15});$.i(1,\'A.3\',4)}f($(1).k(\'5\')||p($(1).k(\'C-5\'))!=\'R\'){$(1).k(\'C-5\',$(1).k(\'5\')||\'\').1e(\'5\')}d 5;f(p 7.5==\'R\'){5=$(1).k(7.5==\'5\'?\'C-5\':7.5)}x f(p 7.5==\'9\'){5=7.5.T(1)}4.1d(\'.3-N\')[7.F?\'F\':\'1c\'](5||7.Z);d 6=$.I({},$(1).H(),{l:1.U,m:1.L});4.1f(0).14=\'3\';4.z().c({b:0,a:0,B:\'12\',P:\'M\'}).11(J.13);d q=4[0].U,t=4[0].L;d j=(p 7.j==\'9\')?7.j.T(1):7.j;1h(j.1q(0)){v\'n\':4.c({b:6.b+6.m,a:6.a+6.l/2-q/2}).r(\'3-1p\');u;v\'s\':4.c({b:6.b-t,a:6.a+6.l/2-q/2}).r(\'3-1r\');u;v\'e\':4.c({b:6.b+6.m/2-t/2,a:6.a-q}).r(\'3-1g\');u;v\'w\':4.c({b:6.b+6.m/2-t/2,a:6.a+6.l}).r(\'3-1o\');u}f(7.G){4.c({K:0,P:\'M\',B:\'Q\'}).1j({K:0.8})}x{4.c({B:\'Q\'})}},9(){$.i(1,\'D.3\',E);d 10=1;1u(9(){f($.i(1,\'D.3\'))o;d 4=$.i(10,\'A.3\');f(7.G){4.1i().1n(9(){$(1).z()})}x{4.z()}},1v)})})};$.h.3.O=9(W,g){o $.V?$.I({},g,$(W).V()):g};$.h.3.Y={G:E,Z:\'\',j:\'n\',F:E,5:\'5\'};$.h.3.1l=9(){o $(1).H().b>($(J).1m()+$(X).m()/2)?\'s\':\'n\'};$.h.3.1t=9(){o $(1).H().a>($(J).1s()+$(X).l()/2)?\'e\':\'w\'}})(1k);',62,94,'|this||tipsy|tip|title|pos|opts||function|left|top|css|var||if|options|fn|data|gravity|attr|width|height||return|typeof|actualWidth|addClass||actualHeight|break|case||else|div|remove|active|visibility|original|cancel|false|html|fade|offset|extend|document|opacity|offsetHeight|block|inner|elementOptions|display|visible|string|class|call|offsetWidth|metadata|ele|window|defaults|fallback|self|appendTo|hidden|body|className|100000|position|true|hover|each|absolute|zIndex|text|find|removeAttr|get|east|switch|stop|animate|jQuery|autoNS|scrollTop|fadeOut|west|north|charAt|south|scrollLeft|autoWE|setTimeout|100'.split('|'),0,{}))

function declOfNum(number, titles)
{
	cases = [2, 0, 1, 1, 1, 2];
	return titles[ (number%100>4 && number%100<20)? 2 : cases[(number%10<5)?number%10:5] ];
}

function nl2br (str)
{
	return str.replace(/([^>])\n/g, '$1<br/>');
}
