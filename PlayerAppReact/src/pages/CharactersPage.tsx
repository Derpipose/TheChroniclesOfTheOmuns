import { useNavigate } from 'react-router-dom'
import NavBar from '../components/NavBar'
import { useCharacters } from '../hooks/useCharacters'


export default function CharactersPage() {
  const navigate = useNavigate()
  const { data: characters, isLoading, isError } = useCharacters()

  return (
    <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
      <NavBar />
      <div className="max-w-3xl mx-auto px-6 py-8">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-semibold" style={{ color: '#2C2416' }}>Your Characters</h2>
          <button
            onClick={() => navigate('/characters/add')}
            // className="bg-amber-500 hover:bg-amber-600 text-white font-medium py-2 px-4 rounded-lg transition duration-200"
            className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
            style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}  
          >
            Add Character
          </button>
        </div>

        {/* Three cases to handle here: */}
        {/* 1. isLoading — show a loading message */}
        {isLoading && (
          <div className="text-center py-20" style={{ color: '#7A6F5C' }}>
            <p className="text-sm">Loading characters...</p>
          </div>
        )}
        {/* 2. isError — show an error message */}
        {isError && (
          <div className="text-center py-20" style={{ color: '#7A6F5C' }}>
            <p className="text-sm">Error occurred while fetching characters.</p>
          </div>
        )}
        {/* 3. otherwise — render the list using `characters` instead of PLACEHOLDER_CHARACTERS */}
        {!isLoading && !isError && (
          <div className="flex flex-col gap-3">
            {characters && characters.length > 0 ? (
              characters.map((char) => (
                <div
                  key={char.id}
                  className="rounded-xl border px-5 py-4 flex items-center justify-between"
                  style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
                >
                  <div className="flex items-center gap-2">
                    <span
                      className="inline-block w-2 h-2 rounded-full flex-shrink-0"
                      style={{ backgroundColor: char.active ? '#5C8A5C' : '#B0A48A' }}
                      title={char.active ? 'Active' : 'In Edit Mode'}
                    />
                    <div>
                      <p className="font-semibold" style={{ color: '#2C2416' }}>{char.name}</p>
                      <p className="text-sm mt-0.5" style={{ color: '#7A6F5C' }}>{char.setting}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <span
                      className="text-xs font-medium px-2.5 py-1 rounded-full"
                      style={{
                        backgroundColor: char.active ? '#E4EEE4' : '#EDE8DC',
                        color: char.active ? '#3D5C3D' : '#5C4F38',
                      }}
                    >
                      {char.active ? 'Active' : 'In Edit Mode'}
                    </span>
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                      Level {char.level}
                    </span>
                    <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
                      {char.maxHealth} HP
                    </span>
                  </div>
                </div>
              ))
            ) : (
              <div className="text-center py-20" style={{ color: '#7A6F5C' }}>
                <p className="text-sm">No characters yet. Add one to get started.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}
// export default function CharactersPage() {
//   const navigate = useNavigate()

//   return (
//     <div className="min-h-screen" style={{ backgroundColor: '#F5F0E8' }}>
//       <NavBar />

//       <div className="max-w-3xl mx-auto px-6 py-8">
//         <div className="flex items-center justify-between mb-6">
//           <h2 className="text-xl font-semibold" style={{ color: '#2C2416' }}>Your Characters</h2>
//           <button
//             onClick={() => navigate('/characters/add')}
//             className="rounded-lg px-4 py-2 text-sm font-semibold transition-opacity hover:opacity-90 active:opacity-75"
//             style={{ backgroundColor: '#8B7355', color: '#FDFAF4' }}
//           >
//             + Add Character
//           </button>
//         </div>

//         {PLACEHOLDER_CHARACTERS.length === 0 ? (
//           <div className="text-center py-20" style={{ color: '#7A6F5C' }}>
//             <p className="text-sm">No characters yet. Add one to get started.</p>
//           </div>
//         ) : (
//           <div className="flex flex-col gap-3">
//             {PLACEHOLDER_CHARACTERS.map((char) => (
//               <div
//                 key={char.id}
//                 className="rounded-xl border px-5 py-4 flex items-center justify-between"
//                 style={{ backgroundColor: '#FDFAF4', borderColor: '#DDD8CC' }}
//               >
//                 <div className="flex items-center gap-2">
//                   <span
//                     className="inline-block w-2 h-2 rounded-full flex-shrink-0"
//                     style={{ backgroundColor: char.active ? '#5C8A5C' : '#B0A48A' }}
//                     title={char.active ? 'Active' : 'In Edit Mode'}
//                   />
//                   <div>
//                     <p className="font-semibold" style={{ color: '#2C2416' }}>{char.name}</p>
//                     <p className="text-sm mt-0.5" style={{ color: '#7A6F5C' }}>{char.setting}</p>
//                   </div>
//                 </div>
//                 <div className="flex items-center gap-3">
//                   <span
//                     className="text-xs font-medium px-2.5 py-1 rounded-full"
//                     style={{
//                       backgroundColor: char.active ? '#E4EEE4' : '#EDE8DC',
//                       color: char.active ? '#3D5C3D' : '#5C4F38',
//                     }}
//                   >
//                     {char.active ? 'Active' : 'In Edit Mode'}
//                   </span>
//                   <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
//                     Level {char.level}
//                   </span>
//                   <span className="text-xs font-medium px-2.5 py-1 rounded-full" style={{ backgroundColor: '#EDE8DC', color: '#5C4F38' }}>
//                     {char.maxHealth} HP
//                   </span>
//                 </div>
//               </div>
//             ))}
//           </div>
//         )}
//       </div>
//     </div>
//   )
// }