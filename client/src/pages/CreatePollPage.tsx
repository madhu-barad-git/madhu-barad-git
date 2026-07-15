import { useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { createPoll } from '../api/polls';
import { getErrorMessage } from '../api/errors';
import './CreatePollPage.css';

const MIN_OPTIONS = 2;
const MAX_OPTIONS = 6;

export default function CreatePollPage() {
  const navigate = useNavigate();
  const [question, setQuestion] = useState('');
  const [options, setOptions] = useState(['', '']);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const updateOption = (index: number, value: string) => {
    setOptions((prev) => prev.map((opt, i) => (i === index ? value : opt)));
  };

  const addOption = () => {
    if (options.length >= MAX_OPTIONS) return;
    setOptions((prev) => [...prev, '']);
  };

  const removeOption = (index: number) => {
    if (options.length <= MIN_OPTIONS) return;
    setOptions((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);

    const trimmedQuestion = question.trim();
    const trimmedOptions = options.map((opt) => opt.trim()).filter((opt) => opt.length > 0);

    if (!trimmedQuestion) {
      setError('Please enter a question.');
      return;
    }

    if (trimmedOptions.length < MIN_OPTIONS || trimmedOptions.length > MAX_OPTIONS) {
      setError(`Please provide between ${MIN_OPTIONS} and ${MAX_OPTIONS} options.`);
      return;
    }

    setIsSubmitting(true);
    try {
      const poll = await createPoll({ question: trimmedQuestion, options: trimmedOptions });
      navigate(`/poll/${poll.id}`);
    } catch (err) {
      setError(getErrorMessage(err, 'Could not create the poll. Please try again.'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page-container">
      <form className="create-poll-form" onSubmit={handleSubmit}>
        <h1>Create a poll</h1>
        <p className="subtitle">Ask a question, add up to 6 options, and share the link.</p>

        <label className="field">
          <span>Question</span>
          <input
            type="text"
            value={question}
            onChange={(e) => setQuestion(e.target.value)}
            placeholder="What should we build next?"
            maxLength={200}
          />
        </label>

        <div className="options-field">
          <span>Options</span>
          {options.map((option, index) => (
            <div className="option-row" key={index}>
              <input
                type="text"
                value={option}
                onChange={(e) => updateOption(index, e.target.value)}
                placeholder={`Option ${index + 1}`}
                maxLength={100}
              />
              <button
                type="button"
                className="remove-option"
                onClick={() => removeOption(index)}
                disabled={options.length <= MIN_OPTIONS}
                aria-label={`Remove option ${index + 1}`}
              >
                &times;
              </button>
            </div>
          ))}

          <button
            type="button"
            className="add-option"
            onClick={addOption}
            disabled={options.length >= MAX_OPTIONS}
          >
            + Add option
          </button>
        </div>

        {error && <div className="form-error">{error}</div>}

        <button type="submit" className="submit-button" disabled={isSubmitting}>
          {isSubmitting ? 'Creating…' : 'Create poll'}
        </button>
      </form>
    </div>
  );
}
