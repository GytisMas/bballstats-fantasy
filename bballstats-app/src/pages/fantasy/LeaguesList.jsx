import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, FormMdContainerStyle, FormMemberNotFullStyle, FormMemberStyle, FormSelectNotFullStyle, FormSelectStyle, FormWiderContainerStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function LeaguesList(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isLoading, setIsLoading] = useState(true);
  const [isTemplLoading, setIsTemplLoading] = useState(true);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRolesUnavailable, setLeagueRolesUnavailable] = useState([]);
  const [leagues, setLeagues] = useState([]);

  const [pageIndex, setPageIndex] = useState(1);
  const [pageCount, setPageCount] = useState(0);
  const [pageNumbers, setPageNumbers] = useState([1, 2, 3]);
  const pageSpan = 5;

  const [sortBy, setSortBy] = useState(0)
  const [activeOnly, setActiveOnly] = useState(true)
  const [nameFilter, setNameFilter] = useState("")

  const navigate = useNavigate();

  // StartDateD = 0,
  // StartDate = 1,
  // EndDateD = 2,
  // EndDate = 3,
  // Name = 4,
  // NameD = 5,



  useEffect(() => {
    const loadData = async () => {
      const response = (await axios.get(APIEndpoint + '/Fantasy/Leagues/?pageIndex=' + pageIndex 
      + '&nameFilter=' + nameFilter
      + '&sortBy=' + sortBy
      + '&activeOnly=' + activeOnly
      ));
      setLeagues(response.data.items);
      setPageCount(response.data.pageCount);
      setPageNumbers(makePageRow(response.data.pageIndex, response.data.pageCount));
    }

    loadData().then(() => setIsLoading(false));
    // console.log("not loading")
    }, [isLoading]);

    const onPageChange = (pageIndex) => {
      setPageIndex(pageIndex)
      setIsLoading(true)
    }

    const onActivityChange = (event) => {
      // document.getElementById("activeOnly").value = event.target.value
      setActiveOnly(event.target.value)
      setIsLoading(true)
    }

    const onSortByChange = (newSortBy) => {
      newSortBy = newSortBy == sortBy 
        ? newSortBy % 2 == 0
          ? newSortBy + 1
          : newSortBy - 1
        : newSortBy
      setSortBy(newSortBy)
      setIsLoading(true)
    }

    const makePageRow = (selectedPage, maxPage) => {
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

    const chevron = (sortByToCheck) => {
      const isDescending = sortBy >= 4 ? sortBy % 2 != 0 : sortBy % 2 == 0
      const fitsValue = sortByToCheck == sortBy || sortByToCheck + 1 == sortBy
      return !fitsValue 
        ? <></>
        : isDescending ? <img className='w-5 inline' src={process.env.PUBLIC_URL + '/icons/down-chevron.svg'}/>
        : <img className='w-5 inline' src={process.env.PUBLIC_URL + '/icons/up-chevron.svg'}/>

    }

    const handleFilter = (clear) => (event) => {
      event.preventDefault()
      const newNameFilter = clear ? "" : document.forms[0].elements["leagueName"].value
      if (clear) {
        document.getElementById("leagueName").value = ""
      }
      setNameFilter(newNameFilter);
      setPageIndex(1)
      setSortBy(0)
      setIsLoading(true)
    } 

    const JoinViewButton = (leagueId) => {
      return <button className={ButtonStyle} type="button" onClick={() => navigate('/fantasy/leagues/'+leagueId+'/participate')}>Join / View</button>;
    }
    
    return (
      <>
        <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-xl text-center'>Fantasy Leagues</p>
        </div>
        <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'>
          <form className='py-5 flex flex-col items-center justify-center'>
            <label>Search by Name</label>
            <input className={FormMemberNotFullStyle} type="text" id="leagueName" name="leagueName" />
            <div>
              <button className={ButtonStyle} type="button" onClick={handleFilter(false)}>Search</button>
              <button className={ButtonStyle} type="button" onClick={handleFilter(true)}>Back to full list</button>
            </div>
          </form>
        </div>
        {isLoading ? <div className='my-5 max-w-xl flex flex-col justify-center items-center mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> :
          <div className='my-5 max-w-xl flex flex-col justify-center items-center mx-auto px-2 bg-white border-2 rounded-3xl'>
            <select id="activeOnly" className={FormSelectNotFullStyle + " mt-5 w-1/2"} onChange={onActivityChange} defaultValue={activeOnly}>
                <option value={false} >All leagues</option>
                <option value={true} >Active leagues</option>
            </select>
            {leagues == null || leagues.length == 0 ? <div>No leagues found.</div>
            :  <>
            <table className="my-2">
              <thead>
                <tr>
                  <th className='p-2 border-2 text-left'><button type="button" onClick={() => onSortByChange(4)}>League Name {chevron(4)}</button></th>
                  <th className='p-2 border-2 text-left'><button type="button" onClick={() => onSortByChange(0)}>Start Date {chevron(0)}</button></th>
                  <th className='p-2 border-2 text-left'><button type="button" onClick={() => onSortByChange(2)}>End Date {chevron(2)}</button></th>
                  <th className='p-2 border-2 text-left'>Actions</th>
                </tr>
              </thead>
              <tbody>
                {leagues.map((league) => (
                  <tr key={league.id} className='w-72 mt-10'>
                    <td className='p-2 border-2 text-left font-semibold hover:font-bold'>
                      <a href={'/fantasy/leagues/'+league.id}>{league.name}</a>
                    </td>
                    <td className='p-2 border-2 text-left'>{new Date(league.startDate).toISOString().substring(0,10)}</td>
                    <td className='p-2 border-2 text-left'>{new Date(league.endDate).toISOString().substring(0,10)}</td>
                    <td className='p-2 border-2 text-left'>{league.isActive ? JoinViewButton(league.id) :  ""}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div>
              <button type="button" className={"mx-2" + (1 == pageIndex ? " font-bold" : "")} onClick={() => onPageChange(1)}>{1}</button>
              {pageNumbers.length > 1 && pageNumbers[1] - pageNumbers[0] > 2 ? <a >...</a> :
                pageNumbers.length > 1 && pageNumbers[1] - pageNumbers[0] == 2 &&
                <button type="button" className={"mx-2" + (2 == pageIndex ? " font-bold" : "")} onClick={() => onPageChange(2)}>{2}</button>
              }
              {pageNumbers.map((page, index) => (
                  index != pageNumbers.length - 1 && index != 0 &&
                  <button key={page} type="button" className={"mx-2" + (page==pageIndex ? " font-bold" : "")} onClick={() => onPageChange(page)}>{page}</button>
              ))
              }
              {pageNumbers.length > 1 && pageNumbers[pageNumbers.length - 1] - pageNumbers[pageNumbers.length - 2] > 2 ? <a>...</a> : 
                pageNumbers.length > 1 && pageNumbers[pageNumbers.length - 1] - pageNumbers[pageNumbers.length - 2] == 2
                && <button type="button" className={"mx-2" + (pageNumbers[pageNumbers.length-1]-1 == pageIndex ? " font-bold" : "")} onClick={() => onPageChange(pageNumbers[pageNumbers.length-1]-1)}>{pageNumbers[pageNumbers.length-1]-1}</button>
              }
              {pageNumbers[pageNumbers.length - 1] > 1 && 
                <button type="button" className={"mx-2" + (pageNumbers[pageNumbers.length-1] == pageIndex ? " font-bold" : "")} onClick={() => onPageChange(pageNumbers[pageNumbers.length-1])}>{pageNumbers[pageNumbers.length-1]}</button>
              }  
            </div></>
            }
            
          </div>
        }
      </>
    );
}

export default LeaguesList;