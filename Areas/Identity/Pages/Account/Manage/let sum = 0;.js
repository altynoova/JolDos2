let sum = 0;
let strNumber = 0;
let number = 0;
while (strNumber !== null) {
  strNumber = prompt("Введите число:", ""); // Получаем из prompt строку либо число в виде строки, если нажата отмена то вернёт null
  number = Number(strNumber); // Приводим строку к числу, если строка не может быть числом то вернёт NaN см. https://learn.javascript.ru/type-conversions
  sum += (!isNaN(number)) ? number : 0; // Использую isNaN потому что Number.isNaN не будет работать в IE у isNaN есть особености см (https://developer.mozilla.org/ru/docs/Web/JavaScript/Reference/Global_Objects/isNaN)
}
if(sum) {
  alert(sum);
}