import { useEffect, useState } from 'react';
import axios from 'axios';
import { APIEndpoint, makePageRow } from "../../components/Helpers";
import { useAuth } from "../../provider/Authentication";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import BearerAuth, { FormulaSecondHalf } from '../../components/Helpers';
import UserGet from '../users/UserGet';
import AlgorithmGet from '../users/AlgorithmGet';

export default function AlgorithmsList() {
  const [algorithms, setAlgorithms] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [statTypes, setStatTypes] = useState([]);

  const [pageIndex, setPageIndex] = useState(1);
  const [pageNumbers, setPageNumbers] = useState([1, 2, 3]);
  const pageSpan = 5;
  
  useEffect(() => {
    const loadAlgorithms = async () => {
      try {
        const response = await axios.get(APIEndpoint + "/customStatistics/?pageIndex=" + pageIndex
        );
        setAlgorithms(response.data.items.sort(function(a, b){return b.id - a.id}));
        setPageNumbers(makePageRow(response.data.pageIndex, response.data.pageCount, pageSpan));

        const statResponse = 
        Object.fromEntries((await axios.get(APIEndpoint + '/statistics/'))
          .data
          .map((stat) => [stat.id, stat.displayName ?? stat.name]));
        setStatTypes(statResponse);
      } catch (error) {
        console.log(error);
      }
      
    };

    loadAlgorithms().then(setIsLoading(false));
  }, [isLoading]);

  const onPageChange = (pageIndex) => {
    setPageIndex(pageIndex)
    setIsLoading(true)
  }
    
  return isLoading ? <div>Loading...</div> : (
    <>
      <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'>
        <p className='py-2 font-bold text-xl text-center'>User-created statistic types</p>
      </div>
      <div className='flex flex-row flex-wrap justify-evenly mt-5 max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
        {algorithms.map((algo) => (
          <div key={algo.id} className='w-72 mt-10'>
            <AlgorithmGet userId={algo.authorId} algoId={algo.id} sTypes={statTypes}/>
          </div>
        ))}
      </div>
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
      </div>
    </>
  );
}