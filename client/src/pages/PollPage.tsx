import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getPoll } from '../api/polls';
import { getErrorMessage } from '../api/errors';
import type { Poll } from '../types';

export default function PollPage() {
  const { id } = useParams<{ id: string }>();
  const [poll, setPoll] = useState<Poll | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    getPoll(id)
      .then(setPoll)
      .catch((err) => setError(getErrorMessage(err, 'Could not load this poll.')));
  }, [id]);

  if (error) return <div className="page-container">{error}</div>;
  if (!poll) return <div className="page-container">Loading…</div>;

  return (
    <div className="page-container">
      <div>
        <h1>{poll.question}</h1>
        <ul>
          {poll.options.map((option) => (
            <li key={option.id}>
              {option.text} — {option.voteCount} vote{option.voteCount === 1 ? '' : 's'}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
