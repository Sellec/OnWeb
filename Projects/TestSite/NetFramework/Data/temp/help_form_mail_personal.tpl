Письмо с сайта в службу персонала<br /><br />

<{if isset($smarty.post.vacancy)}>Желаемая должность: <{$smarty.post.vacancy}><br /><br /><{/if}>

<{if isset($smarty.post.lastname)}>Фамилия: <{$smarty.post.lastname}><br /><{/if}>
<{if isset($smarty.post.firstname)}>Имя: <{$smarty.post.firstname}><br /><{/if}>
<{if isset($smarty.post.middlename)}>Отчество: <{$smarty.post.middlename}><br /><{/if}>
<{if isset($smarty.post.sex)}>Пол: <{$smarty.post.sex}><br /><{/if}>
<{if isset($smarty.post.dateBirth)}>Дата рождения: <{$smarty.post.dateBirth}><br /><{/if}>

<{if isset($smarty.post.email) || isset($smarty.post.phone)}>
	<br />Контактные данные:<br />
	<{if isset($smarty.post.phone)}>Телефон: <a href="tel:<{$smarty.post.phone}>"><{$smarty.post.phone}></a><br /><{/if}>
	<{if isset($smarty.post.email)}>Email: <a href="mailto:<{$smarty.post.email}>"><{$smarty.post.email}></a><br /><{/if}>
	<br />
<{/if}>

<{if isset($smarty.post.education)}>Образование: <{$smarty.post.education}><br /><{/if}>
<{if isset($smarty.post.works)}>Опыт работы: <{$smarty.post.works}><br /><{/if}>

<br />

---
Письмо отправлено с сайта<br />
@Url.ContentFullPath("")<br />
<{$smarty.now|strftime}>