import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, FormMdContainerStyle, FormMemberNotFullStyle, FormMemberStyle, FormSelectNotFullStyle, FormSelectStyle, FormWiderContainerStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint, makePageRow } from "../../components/Helpers";

function Leagues(props) {
  let params = useParams();
  const [isLoading, setIsLoading] = useState(true);
  const [leagues, setLeagues] = useState([]);

  const [pageIndex, setPageIndex] = useState(1);
  const [pageNumbers, setPageNumbers] = useState([1, 2, 3]);
  const pageSpan = 5;

  const [sortBy, setSortBy] = useState(0)
  const [activeOnly, setActiveOnly] = useState(false)
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
      console.log(response.data.items);
      setPageNumbers(makePageRow(response.data.pageIndex, response.data.pageCount, pageSpan));
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
      setPageIndex(1)
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
      return <button className={ButtonStyle} type="button" onClick={() => navigate('/fantasy/leagues/'+leagueId)}>View Standings</button>;
    }
    
    return (
      <>
        <div className='mt-5 max-w-xl flex flex-col items-center mx-auto px-2 bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-lg text-center'>Fantasy League Creation</p>
          <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/create')}>Create League</button>
          <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/templates/create')}>Create Template</button>
        </div>
        {isLoading ? <div className='my-5 max-w-xl flex flex-col justify-center items-center mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> :
          <div className='my-5 max-w-xl flex flex-col justify-center items-center mx-auto px-2 bg-white border-2 rounded-3xl'>
            <p className='py-2 font-bold text-xl text-center'>Browse Fantasy Leagues</p>
            <form className='py-5 flex flex-col items-center justify-center'>
              <label>Search by Name</label>
              <input className={FormMemberNotFullStyle} type="text" id="leagueName" defaultValue={nameFilter} name="leagueName" />
              <div>
                <button className={ButtonStyle} type="button" onClick={handleFilter(false)}>Search</button>
                <button className={ButtonStyle} type="button" onClick={handleFilter(true)}>Back to full list</button>
              </div>
            </form>
            {leagues == null || leagues.length == 0 ? <div>No leagues found.</div>
            :  <>
            <select id="activeOnly" className={FormSelectNotFullStyle + " mt-5 w-1/2"} onChange={onActivityChange} defaultValue={activeOnly}>
                <option value={false} >All leagues</option>
                <option value={true} >Active leagues</option>
            </select>
            <table className="my-2">
              <thead>
                <tr>
                  <th className='p-2 border-2 text-left'><button type="button" onClick={() => onSortByChange(4)}>League Name {chevron(4)}</button></th>
                  <th className='p-2 border-2 text-left'><button className='w-24' type="button" onClick={() => onSortByChange(0)}>Start Date {chevron(0)}</button></th>
                  <th className='p-2 border-2 text-left'><button className='w-24' type="button" onClick={() => onSortByChange(2)}>End Date {chevron(2)}</button></th>
                  <th className='p-2 border-2 text-left'>Actions</th>
                </tr>
              </thead>
              <tbody>
                {leagues.map((league) => (
                  <tr key={league.id} className='w-72 mt-10'>
                    <td className='p-2 border-2 mx-1 text-left'>
                      {league.name}
                    </td>
                    <td className='p-2 border-2 text-left text-nowrap'>{new Date(league.startDate+'z').toISOString().substring(0,10)}</td>
                    <td className='p-2 border-2 text-left text-nowrap'>{new Date(league.endDate+'z').toISOString().substring(0,10)}</td>
                    <td className='p-2 border-2 mx-1 text-left'>{JoinViewButton(league.id)}</td>
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

export default Leagues;