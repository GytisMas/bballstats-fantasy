export function BearerAuth(auth) {
    return 'Bearer ' + auth;
}

export function FormulaFirstHalf(formula) {
  let a = formula.split(') (')[0];
  return a.slice(1, a.length)
}

export function FormulaSecondHalf(formula) {
    return formula.split(') (')[1].slice(0, -1)
}

export const UserRoles = ['Admin', 'Moderator', 'Curator', 'Regular']

export default function isTokenInvalid(token) {
  if (!token)
    return true;
  const expiry = (JSON.parse(atob(token.split('.')[1]))).exp;
  return (Math.floor((new Date()).getTime() / 1000)) >= expiry;
}

export function makePageRow(selectedPage, maxPage, pageSpan) {
  let newPageRow = []
  newPageRow.push(1);
  if (selectedPage != 1 && selectedPage != maxPage) {
    newPageRow.push(selectedPage);
  }
  newPageRow.push(maxPage);
  let positiveShift = selectedPage + 1;
  let negativeShift = selectedPage - 1;

  const actualPageSpan = newPageRow.length == 2 ? pageSpan : pageSpan - 1 

  for (let i = 1; i <= actualPageSpan; i++) {
    const even = i % 2 == 0
    if (even) {
      if (positiveShift >= maxPage) {            
        if (negativeShift <= 1) {
          continue;
        } else {
          newPageRow.splice(1, 0, negativeShift--);
        }
      } else {
        newPageRow.splice(newPageRow.length - 1, 0, positiveShift++);
      }
    }
    else {
      if (negativeShift <= 1) {
        if (positiveShift >= maxPage) {
          continue;
        } else {
          newPageRow.splice(newPageRow.length - 1, 0, positiveShift++);
        }
      } else {
        newPageRow.splice(1, 0, negativeShift--);
      }
    }
  }
  return newPageRow;
}

export const APIEndpoint = 'https://urchin-app-97ttl.ondigitalocean.app/api';
// export const APIEndpoint = 'https://localhost:7140/api';
export const FormSelectStyle = "shadow border rounded w-full py-2 px-2 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
export const FormSelectNotFullStyle = "shadow border rounded py-2 px-2 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
export const FormMemberNotFullStyle = "shadow appearance-none border rounded py-2 px-2 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
export const FormMemberStyle = "shadow appearance-none border rounded w-full py-2 px-2 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
export const FormMemberHalfStyle = "shadow appearance-none border rounded w-1/2 py-2 px-2 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
export const FormTableStyle = "shadow appearance-none border-2 bg-white border-black rounded w-full py-2 px-2 text-gray-700 leading-tight ";
export const FormHelperStyle = "shadow bg-slate-200 mt-2 border rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-blue-200 hover:border-blue-200 leading-tight focus:outline-none focus:shadow-outline";
export const ButtonStyle = "shadow bg-blue-400 mt-2 border rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-blue-600 leading-tight focus:outline-none focus:shadow-outline";
export const LinkStyle = "shadow max-w-min bg-blue-400 mt-2 border border-black rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-blue-600 leading-tight focus:outline-none focus:shadow-outline";
export const FormSumbitStyle = "shadow max-w-min bg-blue-400 mt-2 border rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-blue-600 leading-tight focus:outline-none focus:shadow-outline";
export const FormSumbitStyleForbid = "shadow max-w-min bg-slate-400 mt-2 border rounded py-2 px-3 text-gray-700 transition duration-75 leading-tight focus:outline-none focus:shadow-outline";
export const FormSumbitStyleCancel = "shadow max-w-min bg-slate-400 border rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-slate-600 leading-tight focus:outline-none focus:shadow-outline";
export const FormSumbitStyleCancel2 = "shadow max-w-min bg-red-400 mt-2 border rounded py-2 px-3 text-gray-700 transition duration-75 hover:bg-slate-600 leading-tight focus:outline-none focus:shadow-outline";
export const SubContainerStyle = ' mt-5 duration-75';
export const FormContainerStyle = 'max-w-xs mx-auto mt-5 duration-75';
export const FormWiderContainerStyle = 'max-w-sm mx-auto mt-5 duration-75';
export const Form2XLContainerStyle = 'max-w-2xl mx-auto mt-5 duration-75';
export const Form4XLContainerStyle = 'max-w-6xl mx-auto flex flex-col justify-center mt-5 duration-75';
export const FormMaxWidthContainerStyle = 'max-w-full mx-auto mt-5 duration-75';
export const RoleMemberStyle = "shadow border max-w-sm bg-white rounded w-full py-2 px-3 text-gray-700 leading-tight duration-75 hover:border-blue-400 focus:outline-none focus:shadow-outline";
