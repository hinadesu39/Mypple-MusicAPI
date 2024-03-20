using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDomain.Entity.DTO
{
    public record SearchedResult(Music?[] musics, Album?[] albums, Artist?[] artists);

}
