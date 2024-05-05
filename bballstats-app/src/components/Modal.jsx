import { FormSumbitStyleCancel } from './Helpers';
import './modal.css';

const Modal = ({ show, formContent, handleClose }) => {
  const showHideClassName = show ? "modal display-block" : "modal display-none";

  return (
    <div className={showHideClassName}>
      <section className="modal-main">
          <button type="button" className={FormSumbitStyleCancel + 'float-right'} onClick={handleClose}>
            Close
          </button>
          {formContent}
      </section>
    </div>
  );
};

export default Modal;