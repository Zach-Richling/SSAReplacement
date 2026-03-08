/**
 * Starts a Server-Sent Events stream for job run logs.
 * @param {string} url - Full URL for GET /runs/{id}/logs/stream
 * @param {DotNetObjectReference} dotNetRef - Reference to the Blazor component for callbacks
 * @returns {{ close: () => void }} Handle with close() to stop the stream
 */
export function startJobLogStream(url, dotNetRef) {
  const es = new EventSource(url);

  es.addEventListener('job-log', (e) => {
    try {
      const log = JSON.parse(e.data);
      dotNetRef.invokeMethodAsync('OnLog', log);
    } catch (err) {
      dotNetRef.invokeMethodAsync('OnStreamError', err.message || 'Parse error');
    }
  });

  es.addEventListener('end', () => {
    es.close();
    dotNetRef.invokeMethodAsync('OnStreamEnd');
  });

  es.onerror = () => {
    es.close();
    dotNetRef.invokeMethodAsync('OnStreamError', 'EventSource error');
  };

  return {
    close: () => {
      es.close();
    }
  };
}
