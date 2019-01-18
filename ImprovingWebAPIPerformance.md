#### Improving Web API performance ####

<table>
  <tbody>
    <tr>
      <th>No</th>
      <th>Title</th>
      <th>Description</th>
    </tr>
    <tr>
      <td>1</td>
      <td>Use the fastest JSON serializer available</td>
      <td>Protocol Buffer</td>
    </tr>
    <tr>
      <td>2</td>
      <td>Use compression techniques</td>
      <td>
      <ul>
      <li>Use GZip or Deflate compression on your Web API to compress data transferred between the server and the client.</li>
      <li>Do the compression at the IIS level or by using a custom delegating handler or even using a custom action filter.</li>
      </ul>
      </td>
    </tr>
      <tr>
      <td>3</td>
      <td>Use faster data access strategies</td>
      <td>
      <ul>
      <li>Use classic ADO.Net over ORMs in most cases primarily because using ADO.Net is the fastest way to retrieve data from the database.</li>
      <li>Select the right ORM to suffice your requirements - a lightweight ORM will always fetch data faster compared to ORMs that are complete and are slow.</li>
      </ul>
      </td>
    </tr>
    <tr>
     <td>4</td>
     <td>Use caching</td>
     <td>
     <ul>
     <li>Design Web API in such a way that database hits and multiple requests to Web API are eliminated.</li>
     <li>Cache your Web API methods that are relatively stale - methods that would produce the same response with redundant calls and the response would only change infrequently.</li>
     <li>Cache data that is frequently used and relatively state.</li>
     </ul>
     </td>
    </tr>
     <tr>
      <td>5</td>
      <td>Use asynchronous methods</td>
      <td>Leverage the multiple cores in your system and maximize the application's throughput.</td>
    </tr>
  </tbody>
</table>
